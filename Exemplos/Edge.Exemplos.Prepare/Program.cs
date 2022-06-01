using Newtonsoft.Json;
using System.Data.SQLite;
using System.Diagnostics;

const string insertCommand = "INSERT INTO [Carros](Nome,Preco) VALUES(@nome,@preco)";
string pathRecursos = $@"{Environment.CurrentDirectory}\Recursos";
string pathData = @$"{pathRecursos}\Database.db";
string cs = $@"Data Source={pathData};New=true";
bool created = true;
Stopwatch stopwatch = new();

Carro[] carros = JsonConvert.DeserializeObject<Carro[]>(File.ReadAllText($@"{pathRecursos}\Carros.json")) ?? Array.Empty<Carro>();

if (!File.Exists(pathData))
    created = false;

using var con = new SQLiteConnection(cs);
con.Open();

using var cmd = new SQLiteCommand(insertCommand, con);
stopwatch.Start();
cmd.Prepare();

if (!created)
{
    cmd.CommandText = "CREATE TABLE [Carros](" +
        "[Nome] VARCHAR(500)," +
        "[Preco] MONEY);";
    cmd.ExecuteScalar();

    cmd.CommandText = insertCommand;
    cmd.Prepare();
}

foreach (var carro in carros)
{
    cmd.Parameters.AddWithValue("@nome", carro.Marca);
    cmd.Parameters.AddWithValue("@preco", carro.Preco);
    cmd.ExecuteNonQuery();
}

stopwatch.Stop();
Console.WriteLine($"Tempo de Inserção: {stopwatch.Elapsed}");


class Carro
{
    public string Marca { get; set; }
    public decimal Preco { get; set; }
}