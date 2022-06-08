using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Banco.Edge.Dal.Contas;
public sealed class DaoConta : DaoBase
{
    public async Task<Conta[]?> BuscarContasAsync(int donoId, TipoConta? tipo = null, int pegar = 3)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter("DonoId", donoId),
            new SqlParameter("Take", pegar)
        };

        if (tipo != null)
            parametros.Add(new(nameof(Conta.Tipo), (byte)(tipo ?? TipoConta.Corrente)));

        DataSet data = await ExecuteQueryAsync("BuscarContas", parametros);
        DataRow[] rows = DataTableToRows(data);

        return rows.Length > 0 ? ConverterContas(rows) : null;
    }

    public async Task CriarConta(TipoConta tipo, int donoId)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter("DonoId", donoId),
            new SqlParameter(nameof(Conta.Tipo), (byte)tipo)
        };

        await ExecuteNonQueryAsync("InserirConta", parametros);
    }

    public async Task<Transacao> NovaTransacaoAsync(TipoTransacao tipo, decimal valor, string descricao, int contaId, int? deId, int? referenciaId = null)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter("Tipo", (byte)tipo),
            new SqlParameter("Valor", valor),
            new SqlParameter("Descricao", descricao),
            new SqlParameter("Para", contaId)
        };

        DataSet dataSet = await ExecuteQueryAsync("NovaTransacao", parametros);
        DataRow row = DataTableToRows(dataSet).First();

        int id = row.Field<int>(nameof(Transacao.Id));
        DateTime data = row.Field<DateTime>(nameof(Transacao.Data));

        return new(id, valor, data, tipo, contaId);
    }

    private static Conta[]? ConverterContas(DataRow[] rows)
    {
        Conta[] contas = new Conta[rows.Length];

        for (int i = 0; i < rows.Length; i++)
        {
            DataRow row = rows[i];

            decimal saldo = row.Field<decimal>(nameof(Conta.Saldo));
            int id = row.Field<int>(nameof(Conta.Id));
            TipoConta tipo = (TipoConta)row.Field<byte>(nameof(Conta.Tipo));
            DateTime criacao = row.Field<DateTime>(nameof(Conta.Criacao));
            StatusConta status = (StatusConta)row.Field<byte>(nameof(Conta.Status));

            contas[i] = new(id, saldo, tipo, criacao, status);
        }

        return contas;
    }

    private OrdinalConta GetOrdinalsFromDB()
    {
        SqlConnection connection = null;
        SqlCommand command = null;
        SqlDataReader dr = null;
        OrdinalConta ordinalConta = null;
        try
        {
            String sqlText = "SELECT * FROM Conta";

            connection = new SqlConnection(Resources.ConnectionString);
            command = new SqlCommand(sqlText, conn);

            connection.Open();

            dr = command.ExecuteReader(CommandBehavior.SchemaOnly);
            ordinalConta = new OrdinalConta();
            ordinalConta.OrdinalSaldo = dr.GetOrdinal("Saldo");
            ordinalConta.OrdinalTipo = dr.GetOrdinal("Tipo");
            ordinalConta.OrdinalCriacao = dr.GetOrdinal("Criacao");
            ordinalConta.OrdinalStatus = dr.GetOrdinal("Status");
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
        return ordinalConta;
    }

    public OrdinalConta GetOrdinals()
    {
        return new OrdinalConta
        {
            OrdinalSaldo = 2,
            OrdinalTipo = 3,
            OrdinalStatus = 4,
            OrdinalCriacao = 5
        };
    }

    public string GetContasWithOrdinals()
    {
        OrdinalConta ordinalConta = GetOrdinals();
        StringBuilder sb = new StringBuilder();

        String sqlText = "SELECT * FROM Conta";

        SqlConnection conn = new SqlConnection(Resources.ConnectionString);
        SqlCommand cmd = new SqlCommand(sqlText, conn);
        conn.Open();

        for (int i = 0; i < 1000; i++)
        {
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sb.Append("Saldo: " + dr.GetDecimal(ordinalConta.OrdinalSaldo) + Environment.NewLine +
                    "Tipo: " + dr.GetByte(ordinalConta.OrdinalTipo) + Environment.NewLine +
                    "Criacao: " + dr.GetDateTime(ordinalConta.OrdinalCriacao) + Environment.NewLine +
                    "Status: " + dr.GetByte(ordinalConta.OrdinalStatus) + Environment.NewLine +
                    "=================================================");
            }

            dr.Close();
        }
        conn.Close();

        return sb.ToString();
    }

    public string GetContas()
    {
        StringBuilder sb = new StringBuilder();

        String sqlText = "SELECT * FROM Conta";

        SqlConnection conn = new SqlConnection(Resources.ConnectionString);
        SqlCommand cmd = new SqlCommand(sqlText, conn);
        conn.Open();

        for (int i = 0; i < 1000; i++)
        {
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sb.Append("Saldo: " + dr.GetDecimal("Saldo") + Environment.NewLine +
                    "Tipo: " + dr.GetByte("Tipo") + Environment.NewLine +
                    "Criacao: " + dr.GetDateTime("Criacao") + Environment.NewLine +
                    "Status: " + dr.GetByte("Status") + Environment.NewLine +
                    "=================================================");
            }
            dr.Close();
        }


        conn.Close();

        return sb.ToString();
    }

    public void UpdateConta(Conta contaAtual, Conta novaConta)
    {
        SqlConnection conn = new SqlConnection(Resources.ConnectionString);
        string updateCommandText = "UPDATE Conta SET [Saldo] = @p1, [Tipo] = @p2, [Status] = @p3, [Criacao] = @p4 " +
            "WHERE [Id] = @p5";
        SqlCommand cmd = new SqlCommand(updateCommandText, conn);
        cmd.Parameters.AddWithValue("@p1", novaConta.Saldo);
        cmd.Parameters.AddWithValue("@p2", novaConta.Tipo);
        cmd.Parameters.AddWithValue("@p3", novaConta.Status);
        cmd.Parameters.AddWithValue("@p4", novaConta.Criacao);

        cmd.Parameters.AddWithValue("@p5", contaAtual.Id);

        conn.Open();

        for (int i = 0; i < 1000; i++)
        {
            cmd.ExecuteNonQuery();
        }

        conn.Close();

    }

    public void UpdateContaWithCommandBuilder(Conta contaAtual, Conta novaConta)
    {
        String sqlText = "SELECT * FROM Conta";
        SqlConnection conn = new SqlConnection(Resources.ConnectionString);

        SqlDataAdapter adapter = new SqlDataAdapter(sqlText, conn);
        SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
        builder.QuotePrefix = "[";
        builder.QuoteSuffix = "]";
        Console.WriteLine(builder.GetUpdateCommand().CommandText);

        SqlCommand cmd = new SqlCommand(builder.GetUpdateCommand().CommandText, conn);
        cmd.Parameters.AddWithValue("@p1", -1);
        cmd.Parameters.AddWithValue("@p2", novaConta.Saldo);
        cmd.Parameters.AddWithValue("@p3", novaConta.Tipo);
        cmd.Parameters.AddWithValue("@p4", novaConta.Status);
        cmd.Parameters.AddWithValue("@p5", novaConta.Criacao);
        cmd.Parameters.AddWithValue("@p6", novaConta.Id);

        cmd.Parameters.AddWithValue("@p7", -1);
        cmd.Parameters.AddWithValue("@p8", contaAtual.Id);
        cmd.Parameters.AddWithValue("@p9", contaAtual.Saldo);
        cmd.Parameters.AddWithValue("@p10", contaAtual.Tipo);
        cmd.Parameters.AddWithValue("@p11", contaAtual.Status);
        cmd.Parameters.AddWithValue("@p12", contaAtual.Criacao);

        conn.Open();

        for (int i = 0; i < 1000; i++)
        {
            cmd.ExecuteNonQuery();
        }

        conn.Close();

    }

}