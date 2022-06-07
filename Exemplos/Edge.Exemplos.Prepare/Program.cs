using Newtonsoft.Json;
using System.Data;
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


if (!created)
{
    cmd.CommandText = "CREATE TABLE [Pessoas](" +
        "[Nome] VARCHAR(500) NOT NULL," +
        "[Email] VARCHAR(500) UNIQUE NOT NULL," +
        "[CPF] VARCHAR(15) UNIQUE NOT NULL," +
        "[Idade] INTEGER NOT NULL DEFAULT 1," +
        "[Celular] VARCHAR(16) NOT NULL);";
    cmd.ExecuteNonQuery();

    cmd.CommandText = insertCommand;
}

stopwatch.Start();
for (int i = 0; i < pessoas.Length; i++)
{
    Pessoa? carro = pessoas[i];
    if (i < 1)
    {
        cmd.Parameters.Add(new(nameof(carro.Nome), DbType.StringFixedLength, 500));
        cmd.Parameters.Add(new(nameof(carro.Email), DbType.StringFixedLength, 500));
        cmd.Parameters.Add(new(nameof(carro.CPF), DbType.StringFixedLength, 15));
        cmd.Parameters.Add(new(nameof(carro.Celular), DbType.StringFixedLength, 16));
        cmd.Parameters.Add(new(nameof(carro.Idade), DbType.Int16, 0));
        await cmd.PrepareAsync();
    }

    cmd.Parameters[0].Value = carro.Nome;
    cmd.Parameters[1].Value = carro.Email;
    cmd.Parameters[2].Value = carro.CPF;
    cmd.Parameters[3].Value = carro.Celular;
    cmd.Parameters[4].Value = carro.Idade;

    await cmd.ExecuteNonQueryAsync();
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