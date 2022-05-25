using Banco.Edge.Bll;
using Banco.Edge.Bll.Base;
using Banco.Edge.Dml;
using Newtonsoft.Json;

public static class Program
{
    public static Cliente[] Clientes { get; set; }
    static Random random = new();
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
                Cliente cli = (await BoCliente.BuscarAsync(clientes[i].Email, false)) ?? clientes[i];
                clientes[i] = cli;

                using BoCliente boCliente = new(cli);

                if (cli.Id < 1)
                    cli.Id = await BoCliente.CadastroAsync(cli);

                await boCliente.ObterContasAsync();
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

            Thread th = new(() => Interacoes(start, count));
            th.Start();
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

    public static async void Interacoes(int start, int count)
    {
        Cliente[] clientes = new Cliente[count];

        Array.Copy(Clientes, start, clientes, 0, count);

        while (true)
        {
            int index = random.Next(clientes.Length);
            int transacoes = random.Next(10);

            Conta? conta = clientes[index].Contas?.First();
            if (conta == null)
                continue;

            using BoConta boContaPri = new(conta, clientes[index]);

            if (conta.Saldo < 1.01m * transacoes)
                await boContaPri.DepositarAsync(random.Next(100,1000));

            for (int x = 0; x < transacoes; x++)
            {
                Conta? contaRecebe = clientes[random.Next(clientes.Length)].Contas?.First();

                if (conta == null)
                    continue;

                decimal value = conta.Saldo / (transacoes + 1);

                await boContaPri.TransferirAsync(contaRecebe.Id, value);
            }
        }
    }

    public static int NextInt32(this Random rng)
    {
        int firstBits = rng.Next(0, 1 << 4) << 28;
        int lastBits = rng.Next(0, 1 << 28);
        return firstBits | lastBits;
    }

    public static decimal NextDecimal(this Random rng, decimal? max = null, bool? negative = null)
    {
        byte escala = (byte)rng.Next(29);
        bool assinatura = (negative == null) ? rng.Next(2) == 1 : negative ?? false;
        decimal resultado = new decimal(rng.NextInt32(),
                           rng.NextInt32(),
                           rng.NextInt32(),
                           assinatura,
                           escala);

        while (resultado > max)
            resultado /= 2m;

        return resultado;
    }
}