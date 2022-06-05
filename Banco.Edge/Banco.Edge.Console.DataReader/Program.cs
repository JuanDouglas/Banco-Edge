using Banco.Edge.Bll;
using Banco.Edge.Dml;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Banco.Edge.Cli.DataReader;
public static class Program
{
    public static async Task Main(string[] args)
    {
        Stopwatch stopwatch1 = new();
        Stopwatch stopwatch2 = new();
        using BoCliente boCliente = new(null);
        /*stopwatch1.Start();
        string clientes = boCliente.GetClients();
        stopwatch1.Stop();
        stopwatch2.Start();
        clientes = boCliente.GetClientsWithOrdinals();
        stopwatch2.Stop();
        
        Console.WriteLine($"Tempo de Consulta (sem ordinais): {stopwatch1.Elapsed}");
        Console.WriteLine($"Tempo de Consulta (com ordinais): {stopwatch2.Elapsed}");
        //Console.WriteLine(clientes);*/
        
        Cliente clienteAtual = new Cliente(1, "Felipe", "(88) 88888 8888", "felipe.pontes@edge.ufal.br", "111.222.333-44", "123456", "chave1");
        Cliente novoCliente = new Cliente(1, "Felipe", "(99) 99999 9999", "felipepontes@gmail.com", "111.222.333-44", "654321", "chave2");
        Stopwatch stopwatch3 = new();
        Stopwatch stopwatch4 = new();
        stopwatch3.Start();
        boCliente.UpdateClientWithCommandBuilder(clienteAtual, novoCliente);
        stopwatch3.Stop();
        stopwatch4.Start();
        boCliente.UpdateClient(clienteAtual, novoCliente);
        stopwatch4.Stop();
        Console.WriteLine($"Tempo de Update (com command builder): {stopwatch3.Elapsed}");
        Console.WriteLine($"Tempo de Update (sem command builder): {stopwatch4.Elapsed}");
    }

    

}