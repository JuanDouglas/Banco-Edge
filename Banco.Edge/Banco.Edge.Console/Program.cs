// See https://aka.ms/new-console-template for more information
using Banco.Edge.Bll;
using Banco.Edge.Bll.Base;
using Banco.Edge.Dml;
using Newtonsoft.Json;
using System.IO;

public static class Program
{
    public static async Task Main(string[] args)
    {
        string path = Environment.CurrentDirectory;
        path = Path.Combine(path, "Pessoas");
        var files = Directory.GetFiles(path);

        foreach (var file in files)
        {
            Cliente[] clientes = ObterClientes(file, out string[] senhas);

            for (int i = 0; i < clientes.Length; i++)
            {
                Cliente cli = (await BoCliente.BuscarAsync(clientes[i].Email, false)) ?? clientes[i];

                if (cli.Id > 0)
                {
                    BoCliente boCliente = new(cli);

                    await boCliente.ExcluirAsync(senhas[i]);
                }

                cli.Id = await BoCliente.CadastroAsync(cli);
            }

            GC.SuppressFinalize(clientes);
            GC.Collect();
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
}