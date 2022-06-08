using Banco.Edge.Bll;
using Banco.Edge.Bll.Base;
using Banco.Edge.Dml;
using Newtonsoft.Json;

namespace Banco.Edge.Cli;
public static class Program
{
    public static Cliente[] Clientes { get; set; }
    static byte[][]? imagens;
    static Random rd = Random.Shared;
    public static async Task Main(string[] args)
    {
        List<Cliente> clis = new();
        string path = Environment.CurrentDirectory;
        path = Path.Combine(path, "Pessoas");

        var files = Directory.GetFiles(path);
        imagens = await GetImagens(path);

        Console.WriteLine($"Carregando clientes de {files.Length} arquivos.");

        foreach (var file in files)
        {
            Cliente[] clientes = ObterClientes(file, out string[] senhas);

            for (int i = 0; i < clientes.Length; i++)
            {
                Cliente cli = await BoCliente.BuscarAsync(clientes[i].Email, false) ?? clientes[i];
                clientes[i] = cli;

                using BoCliente boCliente = new(cli);

                if (cli.Id < 1)
                    cli.Id = await BoCliente.CadastroAsync(cli);

                await boCliente.ObterContasAsync();

                Conta? conta = cli.Contas?.First();
                if (conta == null)
                    continue;

                if (conta.Saldo < 1m)
                {
                    using BoConta boConta = new(conta, cli);
                    decimal valor = rd.Next(10, 200);
                    await boConta.DepositarAsync(valor);
                }
            }

            clis.AddRange(clientes);
        }

    }

    private static async Task<byte[][]> GetImagens(string path)
    {
        var files = Directory.GetFiles(Path.Combine(path, "Fotos"));

        List<byte[]> imagens = new();
        foreach (var item in files)
        {
            imagens.Add(await File.ReadAllBytesAsync(item));
        }

        return imagens.ToArray();
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
                imagens ??= new byte[][] { Array.Empty<byte>() };
                cliente.Foto = imagens[rd.Next(0, imagens.Length)];
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
}