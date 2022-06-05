using Newtonsoft.Json;
using System.Data.SQLite;
using System.Diagnostics;

public static class Program
{
    public static async Task Main(string[] args)
    {
        for (int i = 0; i < 10; i++)
        {
            InsertPessoas(false);
            InsertPessoas(true);
        }
    }

    public static void InsertPessoas(bool prepare)
    {
        string pathRecursos = $@"{Environment.CurrentDirectory}\Recursos";
        string pathData = @$"{pathRecursos}\Database.db";
        string cs = $@"Data Source={pathData};New=true";
        bool created = true;
        Stopwatch stopwatch = new();

        Pessoa[] pessoas = JsonConvert.DeserializeObject<Pessoa[]>(File.ReadAllText($@"{pathRecursos}\Pessoas.json")) ?? Array.Empty<Pessoa>();

        if (!File.Exists(pathData))
            created = false;

        using var con = new SQLiteConnection(cs);
        con.Open();

        using var cmd = new SQLiteCommand(null, con);

        if (!created)
        {
            using var cmdCreate = new SQLiteCommand(con);
            cmdCreate.CommandText = "CREATE TABLE [Pessoas](" +
                "[Nome] VARCHAR(500) NOT NULL," +
                "[Email] VARCHAR(500) NOT NULL," +
                "[CPF] VARCHAR(15) NOT NULL," +
                "[Idade] INTEGER NOT NULL DEFAULT 1," +
                "[Celular] VARCHAR(16) NOT NULL);";
            cmdCreate.ExecuteScalar();

        }

        const string insertCommand = "INSERT INTO [Pessoas](Nome,Email,CPF,Celular,Idade) VALUES(@nome,@email,@cpf,@celular,@idade)";
        cmd.CommandText = insertCommand;

        stopwatch.Start();
        if (prepare)
            cmd.Prepare();

        for (int i = 0; i < 50; i++)
        {
            foreach (var pessoa in pessoas)
            {
                cmd.Parameters.AddWithValue("@nome", pessoa.Nome);
                cmd.Parameters.AddWithValue("@email", pessoa.Email);
                cmd.Parameters.AddWithValue("@cpf", pessoa.CPF);
                cmd.Parameters.AddWithValue("@celular", pessoa.Celular);
                cmd.Parameters.AddWithValue("@idade", pessoa.Idade);
                cmd.ExecuteNonQuery();
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"Tempo de Inserção (prepare={prepare}): {stopwatch.Elapsed}");
    }
}

class Pessoa
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public string Celular { get; set; }
    public int Idade { get; set; }
}