using Banco.Edge.Bll;
using System.Diagnostics;

namespace Banco.Edge.Cli. Retrieval;
public static class Program
{
    public static async Task Main(string[] args)
    {

        Stopwatch stopwatch1 = new();
        Stopwatch stopwatch2 = new();
        Stopwatch stopwatch3 = new();
        Stopwatch stopwatch4 = new();

        int qtd = 100;

        using BoCliente boCliente = new(null);
        string sql1 = "SELECT * FROM Cliente";

        stopwatch1.Start();
        boCliente.RecuperarClientes(sql1, qtd);
        stopwatch1.Stop();
        Console.WriteLine($"Tempo da Consulta {sql1}: {stopwatch1.Elapsed}");

        string sql2 = "SELECT * FROM Cliente WHERE Id > 100 AND Id < 200";
        stopwatch2.Start();
        boCliente.RecuperarClientes(sql2, qtd);
        stopwatch2.Stop();
        Console.WriteLine($"Tempo da Consulta {sql2}: {stopwatch2.Elapsed}");

        string sql3 = "SELECT Nome FROM Cliente";

        stopwatch3.Start();
        boCliente.RecuperarClientes(sql3, qtd);
        stopwatch3.Stop();
        Console.WriteLine($"Tempo da Consulta {sql3}: {stopwatch3.Elapsed}");

        string sql4 = "SELECT Nome FROM Cliente WHERE Id > 100 AND Id < 200";

        stopwatch4.Start();
        boCliente.RecuperarClientes(sql4, qtd);
        stopwatch4.Stop();
        Console.WriteLine($"Tempo da Consulta {sql4}: {stopwatch4.Elapsed}");
    }
}