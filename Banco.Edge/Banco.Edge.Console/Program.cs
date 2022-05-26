using Banco.Edge.Bll;
using Banco.Edge.Bll.Base;
using Banco.Edge.Dml;
using Newtonsoft.Json;

namespace Banco.Edge.Cli;
public static class Program
{
    public static Cliente[] Clientes { get; set; }
    static Random rd = Random.Shared;
    public static async Task Main(string[] args)
    {
        List<Cliente> clis = new();
        string path = Environment.CurrentDirectory;
        path = Path.Combine(path, "Pessoas");
        var files = Directory.GetFiles(path);
        Console.WriteLine($"Carregando clientes de {files.Length} arquivos.");

        foreach (var file in files)
        {
            Cliente[] clientes = ObterClientes(file, out string[] senhas);

            for (int i = 0; i < clientes.Length; i++)
            {
                Cliente cli = await BoCliente.BuscarAsync(clientes[i].Email, false) ?? clientes[i];
                clientes[i] = cli;

                BoCliente boCliente = new(cli);

                if (cli.Id < 1)
                    cli.Id = await BoCliente.CadastroAsync(cli);

                await boCliente.ObterContasAsync();

                Conta? conta = cli.Contas?.First();
                if (conta == null)
                    continue;

                if (conta.Saldo < 1m)
                {
                    BoConta boConta = new(conta, cli);
                    decimal valor = rd.Next(10, 200);
                    await boConta.DepositarAsync(valor);
                }
            }

            clis.AddRange(clientes);
        }

        Clientes = clis.ToArray();
        Console.Write("Digite o numero de interações que deseja fazer: ");
        _ = int.TryParse(Console.ReadLine(), out int interacoes);

        for (int i = 1; i <= interacoes; i++)
        {
            int count = Clientes.Length / interacoes;
            int start = count * (i - 1);

            Thread th = new(() => Transferencias(start, count));
            th.Start();

            if (i % 3 == 0)
            {
                Thread depositos = new(() => Depositos());
                depositos.Start();
            }
        }
    }

    public static Cliente[] ObterClientes(string file, out string[] senhas)
    {
        List<Cliente> clientes = new();
        List<string> listSenhas = new();
        FileStream stream = new(file, FileMode.Open, FileAccess.Read);
        using JsonReader reader = new JsonTextReader(new StreamReader(stream));

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndArray)
                break;

            if (reader.TokenType != JsonToken.StartObject)
                reader.Read();

            Cliente? cliente = ConverterCliente(reader, out string senha);

            if (cliente != null)
            {
                clientes.Add(cliente);
                listSenhas.Add(senha);
            }
        }

        senhas = listSenhas.ToArray();
        return clientes.ToArray();
    }
    private static Cliente? ConverterCliente(JsonReader reader, out string senha)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
        string
            nome = string.Empty,
            email = string.Empty,
            cpf = string.Empty,
            celular = string.Empty,
            chave = BoBase.GerarToken(96);
        bool finalized = false;
        senha = string.Empty;

        try
        {
            while (!finalized)
            {
                reader.Read();
                switch (reader.Value ?? string.Empty)
                {
                    case nameof(nome):
                        nome = reader.ReadAsString();
                        break;

                    case nameof(email):
                        email = reader.ReadAsString();
                        break;

                    case nameof(cpf):
                        cpf = reader.ReadAsString();
                        break;

                    case nameof(celular):
                        celular = reader.ReadAsString();
                        break;

                    case nameof(senha):
                        senha = reader.ReadAsString();
                        break;
                }

                finalized = reader.TokenType == JsonToken.EndObject;
            }
        }
        catch (Exception)
        {
            return null;
        }

        return new(0, nome, celular, email, cpf, senha, chave);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }
    public static async void Transferencias(int start, int count)
    {
        Cliente[] clientes = new Cliente[count];

        Array.Copy(Clientes, start, clientes, 0, count);

        while (true)
        {
            int index = rd.Next(clientes.Length);
            int transacoes = rd.Next(10);

            Conta? conta = clientes[index].Contas?.First();
            if (conta == null)
                continue;

            BoConta boContaPri = new(conta, clientes[index]);

            for (int x = 0; x < transacoes; x++)
            {
                Conta? contaRecebe = clientes[rd.Next(clientes.Length)].Contas?.First();

                decimal peso = rd.Next(10, 50);

                if (conta == null ||
                    conta.Saldo / peso < 1m)
                    continue;

                decimal value = conta.Saldo / peso;

                await boContaPri.TransferirAsync(contaRecebe.Id, value);
            }
        }
    }
    public static async void Depositos()
    {
        while (true)
        {

            int index = rd.Next(Clientes.Length);
            Cliente cliente = Clientes[index];

            Conta? conta = cliente.Contas?.First();

            if (conta == null)
                continue;

            BoConta boConta = new(conta, cliente);

            decimal valorTotal = rd.Next((int)Math.Round(2d * 50 * 0.8), 1000) * 0.8m;
            int depositos = rd.Next(50);

            for (int i = 0; i < depositos; i++)
                _ = await boConta.DepositarAsync(valorTotal / depositos);

            Thread.Sleep(rd.Next(0, 10000));
        }
    }
}