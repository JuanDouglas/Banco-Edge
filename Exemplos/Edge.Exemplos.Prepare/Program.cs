using Newtonsoft.Json;
using System.Data.SQLite;
using System.Diagnostics;

const string insertCommand = "INSERT INTO [Pessoas](Nome,Email,CPF,Celular,Idade) VALUES(@nome,@email,@cpf,@celular,@idade)";
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

using var cmd = new SQLiteCommand(insertCommand, con);
stopwatch.Start();

if (!created)
{
    cmd.CommandText = "CREATE TABLE [Pessoas](" +
        "[Nome] VARCHAR(500) NOT NULL," +
        "[Email] VARCHAR(500) UNIQUE NOT NULL," +
        "[CPF] VARCHAR(15) UNIQUE NOT NULL," +
        "[Idade] INTEGER NOT NULL DEFAULT 1," +
        "[Celular] VARCHAR(16) NOT NULL);";
    cmd.ExecuteScalar();

    cmd.CommandText = insertCommand;
}

foreach (var carro in pessoas)
{
    cmd.Parameters.AddWithValue("@nome", carro.Nome);
    cmd.Parameters.AddWithValue("@email", carro.Email);
    cmd.Parameters.AddWithValue("@cpf", carro.CPF);
    cmd.Parameters.AddWithValue("@celular", carro.Celular);
    cmd.Parameters.AddWithValue("@idade", carro.Idade);
    cmd.ExecuteNonQuery();
}

stopwatch.Stop();
Console.WriteLine($"Tempo de Inserção: {stopwatch.Elapsed}");


class Pessoa
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public string Celular { get; set; }
    public int Idade { get; set; }
}