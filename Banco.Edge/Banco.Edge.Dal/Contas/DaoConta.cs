using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using System.Data.SqlClient;

namespace Banco.Edge.Dal.Contas;
public sealed class DaoConta : DaoBase
{
    public async Task<Conta> BuscarContaAsync(int donoId, TipoConta tipo)
    {
        List<SqlParameter> parametros = new()
        {

        };


        throw new NotImplementedException();
    }

    public async Task CriarConta(TipoConta tipo, int donoId)
    {
        throw new NotImplementedException();
    }

    public async Task<Conta[]> ObterContasAsync(int donoId)
    {
        throw new NotImplementedException();
    }
}