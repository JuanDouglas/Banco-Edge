using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;

namespace Banco.Edge.Dal.Contas;
public sealed class DaoConta : DaoBase
{
    public async Task<Conta> BuscarContaAsync(int donoId, TipoConta tipo)
    {
        throw new NotImplementedException();
    }

    public async Task CriarConta(TipoConta tipo, int donoId)
    {
        throw new NotImplementedException();
    }
}