using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Dal.Contas;
public sealed class DaoConta : DaoBase
{
    public async Task<Conta[]?> BuscarContasAsync(int donoId, TipoConta? tipo = null, int pegar = 3)
    {
        SqlParameter[] parametros =
        {
            new("DonoId", donoId),
            new("Take", pegar)
        };

        if (tipo != null)
        {
            SqlParameter[] aux = new SqlParameter[parametros.Length + 1];
            aux[0] = new(nameof(Conta.Tipo), (byte)(tipo ?? TipoConta.Corrente));
            parametros.CopyTo(aux, 1);
            parametros = aux;
        }

        DataSet data = await ExecuteQueryAsync("BuscarContas", parametros);
        DataRow[] rows = DataTableToRows(data);

        return rows.Length > 0 ? ConverterContas(rows) : null;
    }

    public async Task CriarConta(TipoConta tipo, int donoId)
    {
        SqlParameter[] parametros =
         {
            new("DonoId", donoId),
            new(nameof(Conta.Tipo), (byte)tipo)
        };

        await ExecuteNonQueryAsync("InserirConta", parametros);
    }

    public async Task<Transacao> NovaTransacaoAsync(TipoTransacao tipo, decimal valor, string descricao, int contaId, int? deId, int? referenciaId = null)
    {
        SqlParameter[] parametros =
        {
            new("Tipo", (byte)tipo),
            new("Valor", valor),
            new("Descricao", descricao),
            new("Para", contaId)
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
}