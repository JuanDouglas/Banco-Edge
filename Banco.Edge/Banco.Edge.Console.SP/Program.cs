using Banco.Edge.Bll;
using System.Diagnostics;

namespace Banco.Edge.Cli.SP;
public static class Program
{
    public static async Task Main(string[] args)
    {

        Stopwatch stopwatch1 = new();
        Stopwatch stopwatch2 = new();

        int qtd = 200;

        using BoCliente boCliente = new(null);
        string sp1 = "BuscaCliente(skip=0, take=1000)";

        stopwatch1.Start();
        boCliente.RecuperarClientesSpLiteral(sp1, qtd);
        stopwatch1.Stop();
        Console.WriteLine($"Tempo da SP (literal) {sp1}: {stopwatch1.Elapsed}");

        string sp2 = "BuscaCliente";
        stopwatch2.Start();
        boCliente.RecuperarClientesSp(sp2, qtd);
        stopwatch2.Stop();
        Console.WriteLine($"Tempo da SP (RPC) {sp2}: {stopwatch2.Elapsed}");

    }
}