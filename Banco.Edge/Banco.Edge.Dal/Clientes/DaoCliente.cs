using Banco.Edge.Dml;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Banco.Edge.Dal.Clientes;

public sealed class DaoCliente : DaoBase
{
    public async Task<Cliente?> ExisteAsync(string? email, string? cpfOuCnpj, bool ocultarSensiveis = true)
    {
        if (string.IsNullOrEmpty(cpfOuCnpj) &&
            string.IsNullOrEmpty(email))
            throw new ArgumentNullException(nameof(cpfOuCnpj), "Ao menos um parametro deve conter valor!");

        Cliente[] clientes = await ExecutarBuscaAsync(email: email, cpfOuCnpj: cpfOuCnpj, ocultarSensiveis: ocultarSensiveis);

        return clientes.Length < 1 ? null : clientes[0];
    }

    public async Task ExcluirAsync(int id)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter("IdCliente", id)
        };

        await ExecuteQueryAsync("ExcluirCliente", parametros, true);
    }

    public async Task<int> InserirCliente(Cliente cliente)
    {
        List<SqlParameter> parameters = new()
        {
            new SqlParameter(nameof(Cliente.Nome), cliente.Nome),
            new SqlParameter(nameof(Cliente.Email), cliente.Email),
            new SqlParameter(nameof(Cliente.Senha), cliente.Senha),
            new SqlParameter(nameof(Cliente.Chave), cliente.Chave),
            new SqlParameter(nameof(Cliente.Telefone), cliente.Telefone),
            new SqlParameter(nameof(Cliente.CpfOuCnpj), cliente.CpfOuCnpj)
        };

        DataSet dbSet = await ExecuteQueryAsync("InserirCliente", parameters, true);
        DataRowCollection rows = dbSet.Tables[0].Rows;

        int id = -1;

        if (rows.Count > 0)
            _ = int.TryParse(rows[0][0].ToString(), out id);

        return id;
    }

    private async Task<Cliente[]> ExecutarBuscaAsync(
        int? id = null,
        string? email = null,
        string? cpfOuCnpj = null,
        int take = 1,
        int skip = 0,
        bool ocultarSensiveis = true)
    {
        List<SqlParameter> parameters = new()
        {
            new("Skip", skip),
            new("Take", take),
            new(nameof(Cliente.Id), id),
            new(nameof(Cliente.Email), email),
            new(nameof(Cliente.CpfOuCnpj), cpfOuCnpj)
        };

        DataSet ds = await ExecuteQueryAsync("BuscaCliente", parameters);

        DataRow[] rows = DataTableToRows(ds);

        return Converter(rows, ocultarSensiveis);
    }
    private Cliente[] Converter(DataRow[] rows, bool ocultarSensiveis)
    {
        Cliente[] clientes = new Cliente[rows.Length];

        for (int i = 0; i < rows.Length; i++)
        {
            DataRow row = rows[i];

            int id = row.Field<int>(nameof(Cliente.Id));
            string nome = row.Field<string>(nameof(Cliente.Nome)) ?? string.Empty;
            string email = row.Field<string>(nameof(Cliente.Email)) ?? string.Empty;
            string cpfOuCnpj = row.Field<string>(nameof(Cliente.CpfOuCnpj)) ?? "00000000000";
            string telefone = row.Field<string>(nameof(Cliente.Telefone)) ?? "+00 (00) 00000-0000";
            string chave = row.Field<string>(nameof(Cliente.Chave)) ?? string.Empty;
            string senha = row.Field<string>(nameof(Cliente.Senha)) ?? string.Empty;

            clientes[i] = new(id, nome, telefone, email, cpfOuCnpj, senha, chave, ocultarSensiveis);
        }

        return clientes;
    }

    private OrdinalCliente GetOrdinalsFromDB()
    {
        SqlConnection connection = null;
        SqlCommand command = null;
        SqlDataReader dr = null;
        OrdinalCliente ordinalCliente = null;
        try
        {
            String sqlText = "SELECT * FROM Cliente";

            connection = new SqlConnection(Resources.ConnectionString);
            command = new SqlCommand(sqlText, conn);

            connection.Open();

            dr = command.ExecuteReader(CommandBehavior.SchemaOnly);
            ordinalCliente = new OrdinalCliente();
            ordinalCliente.OrdinalNome = dr.GetOrdinal("Nome");
            ordinalCliente.OrdinalEmail = dr.GetOrdinal("Email");
            ordinalCliente.OrdinalTelefone = dr.GetOrdinal("Telefone");
            ordinalCliente.OrdinalCpf = dr.GetOrdinal("CpfOuCnpj");
            ordinalCliente.OrdinalSenha = dr.GetOrdinal("Senha");
            ordinalCliente.OrdinalChave = dr.GetOrdinal("Chave");
            ordinalCliente.OrdinalCadastro = dr.GetOrdinal("Cadastro");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            if (dr != null)
                dr.Close();
            if (connection != null)
                connection.Close();
        }
        return ordinalCliente;
    }

    public OrdinalCliente GetOrdinals()
    {
        return new OrdinalCliente
        {
            OrdinalNome = 1,
            OrdinalEmail = 2,
            OrdinalTelefone = 3,
            OrdinalCpf = 4,
            OrdinalSenha = 5,
            OrdinalChave = 6,
            OrdinalCadastro = 7
        };
    }

    public string GetClientsWithOrdinals()
    {
        OrdinalCliente ordinalCliente = GetOrdinals();
        StringBuilder sb = new StringBuilder();

        String sqlText = "SELECT * FROM Cliente";

        SqlConnection conn = new SqlConnection(Resources.ConnectionString);
        SqlCommand cmd = new SqlCommand(sqlText, conn);
        conn.Open();

        for (int i = 0; i < 1000; i++)
        {
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sb.Append("Nome: " + dr.GetString(ordinalCliente.OrdinalNome) + Environment.NewLine +
                    "Email: " + dr.GetString(ordinalCliente.OrdinalEmail) + Environment.NewLine +
                    "Telefone: " + dr.GetString(ordinalCliente.OrdinalTelefone) + Environment.NewLine +
                    "Cpf: " + dr.GetString(ordinalCliente.OrdinalCpf) + Environment.NewLine +
                    "Senha: " + dr.GetString(ordinalCliente.OrdinalSenha) + Environment.NewLine +
                    "Chave: " + dr.GetString(ordinalCliente.OrdinalChave) + Environment.NewLine +
                    "Cadastro: " + dr.GetDateTime(ordinalCliente.OrdinalCadastro) + Environment.NewLine +
                    "=================================================");
            }

            dr.Close();
        }
        conn.Close();

        return sb.ToString();
    }

    public string GetClients()
    {
        StringBuilder sb = new StringBuilder();

        String sqlText = "SELECT * FROM Cliente";

        SqlConnection conn = new SqlConnection(Resources.ConnectionString);
        SqlCommand cmd = new SqlCommand(sqlText, conn);
        conn.Open();

        for (int i = 0; i < 1000; i++)
        {
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sb.Append(
                "Nome: " + dr.GetString("Nome") + Environment.NewLine +
                    "Email: " + dr.GetString("Email") + Environment.NewLine +
                    "Telefone: " + dr.GetString("Telefone") + Environment.NewLine +
                    "Cpf: " + dr.GetString("CpfOuCnpj") + Environment.NewLine +
                    "Senha: " + dr.GetString("Senha") + Environment.NewLine +
                    "Chave: " + dr.GetString("Chave") + Environment.NewLine +
                    "Cadastro: " + dr.GetDateTime("Cadastro") + Environment.NewLine +
                    "=================================================");
            }
            dr.Close();
        }

        
        conn.Close();

        return sb.ToString();
    }

    public void UpdateClient(Cliente clienteAtual, Cliente novoCliente)
    {
        SqlConnection conn = new SqlConnection(Resources.ConnectionString);
        string updateCommandText = "UPDATE Cliente SET [Nome] = @p1, [Email] = @p2, [Telefone] = @p3, [CpfOuCnpj] = @p4, " +
            "[Senha] = @p5, [Chave] = @p6, [Cadastro] = @p7 WHERE [Id] = @p8";
        SqlCommand cmd = new SqlCommand(updateCommandText, conn);
        cmd.Parameters.AddWithValue("@p1", novoCliente.Nome);
        cmd.Parameters.AddWithValue("@p2", novoCliente.Email);
        cmd.Parameters.AddWithValue("@p3", novoCliente.Telefone);
        cmd.Parameters.AddWithValue("@p4", novoCliente.CpfOuCnpj);
        cmd.Parameters.AddWithValue("@p5", novoCliente.Senha);
        cmd.Parameters.AddWithValue("@p6", novoCliente.Chave);
        cmd.Parameters.AddWithValue("@p7", DateTime.Now);

        cmd.Parameters.AddWithValue("@p8", clienteAtual.Id);
        conn.Open();

        for (int i = 0; i < 1000; i++)
        {
            cmd.ExecuteNonQuery();
        }

        conn.Close();

    }

    public void UpdateClientWithCommandBuilder(Cliente clienteAtual, Cliente novoCliente)
    {
        String sqlText = "SELECT * FROM Cliente";
        SqlConnection conn = new SqlConnection(Resources.ConnectionString);

        SqlDataAdapter adapter = new SqlDataAdapter(sqlText, conn);
        SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
        builder.QuotePrefix = "[";
        builder.QuoteSuffix = "]";
        Console.WriteLine(builder.GetUpdateCommand().CommandText);

        SqlCommand cmd = new SqlCommand(builder.GetUpdateCommand().CommandText, conn);
        cmd.Parameters.AddWithValue("@p1", novoCliente.Nome);
        cmd.Parameters.AddWithValue("@p2", novoCliente.Email);
        cmd.Parameters.AddWithValue("@p3", novoCliente.Telefone);
        cmd.Parameters.AddWithValue("@p4", novoCliente.CpfOuCnpj);
        cmd.Parameters.AddWithValue("@p5", novoCliente.Senha);
        cmd.Parameters.AddWithValue("@p6", novoCliente.Chave);
        cmd.Parameters.AddWithValue("@p7", DateTime.Now);

        cmd.Parameters.AddWithValue("@p8", clienteAtual.Id);
        cmd.Parameters.AddWithValue("@p9", clienteAtual.Nome);
        cmd.Parameters.AddWithValue("@p10", clienteAtual.Email);
        cmd.Parameters.AddWithValue("@p11", clienteAtual.Telefone);
        cmd.Parameters.AddWithValue("@p12", clienteAtual.CpfOuCnpj);
        cmd.Parameters.AddWithValue("@p13", clienteAtual.Senha);
        cmd.Parameters.AddWithValue("@p14", clienteAtual.Chave);
        cmd.Parameters.AddWithValue("@p15", DateTime.Now);

        conn.Open();

        for (int i = 0; i < 1000; i++)
        {
            cmd.ExecuteNonQuery();
        }

        conn.Close();

    }
 

}