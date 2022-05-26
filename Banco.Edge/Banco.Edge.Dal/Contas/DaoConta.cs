using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Dal.Contas;
public sealed class DaoConta : DaoBase
{
    public Conta[]? BuscarContasAsync(int donoId, TipoConta? tipo = null, int pegar = 3)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter("DonoId", donoId),
            new SqlParameter("Take", pegar)
        };

        if (tipo != null)
            parametros.Add(new(nameof(Conta.Tipo), (byte)(tipo ?? TipoConta.Corrente)));

        DataSet data = ExecuteQuery("BuscarContas", parametros);
        DataRow[] rows = DataTableToRows(data);

        return rows.Length > 0 ? ConverterContas(rows) : null;
    }

    public void CriarConta(TipoConta tipo, int donoId)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter("DonoId", donoId),
            new SqlParameter(nameof(Conta.Tipo), (byte)tipo)
        };

        ExecuteNonQuery("InserirConta", parametros);
    }

    public Transacao NovaTransacao(TipoTransacao tipo, decimal valor, string descricao, int contaId, int? deId, int? referenciaId = null)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter("Tipo", (byte)tipo),
            new SqlParameter("Valor", valor),
            new SqlParameter("Descricao", descricao),
            new SqlParameter("Para", contaId)
        };

        DataSet dataSet =  ExecuteQuery("NovaTransacao", parametros);
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
}