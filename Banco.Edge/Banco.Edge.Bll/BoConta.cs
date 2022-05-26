using Banco.Edge.Bll.Base;
using Banco.Edge.Dal.Contas;
using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;

namespace Banco.Edge.Bll;

public class BoConta : BoBase
{
    private const string descricaoTransferencia = "Transferência não especificada!";
    private const string descricaoDeposito = "Depoisto sem descrição";
    public const decimal maximoValorMoney = 922337203685477.56m;
    public const decimal minimoTransacao = 1m;

    public Cliente Cliente { get; set; }
    public Conta Conta { get; set; }
    private protected DaoConta DaoConta { get; set; }
    public BoConta(Conta conta, Cliente cliente)
    {
        DaoConta = new();
        Cliente = cliente;
        Conta = conta;
    }

    public async Task TransferirAsync(int idConta, decimal valor, string? descricao = null)
    {
        if (valor < minimoTransacao ||
            valor > maximoValorMoney)
            throw new ArgumentException();

        if (Conta.Saldo < valor)
            throw new ArgumentException();

        valor = decimal.Round(valor, 2);
        await DaoConta.NovaTransacaoAsync(TipoTransacao.Transferencia, valor, descricao ?? descricaoTransferencia, idConta, Conta.Id);

        Conta.Saldo -= valor;
    }

    public async Task<Transacao> DepositarAsync(decimal valor, string? descricao = null)
    {
        if (valor < minimoTransacao ||
            valor > maximoValorMoney)
            throw new ArgumentException();

        valor = decimal.Round(valor, 2);

        Transacao transacao = await DaoConta.NovaTransacaoAsync(TipoTransacao.Deposito, valor, descricao ?? descricaoDeposito, Conta.Id, null);

        Conta.Saldo += transacao.Valor;

        return transacao;
    }

    public override void Dispose()
    {
        DaoConta.Dispose();
        GC.Collect();
        GC.SuppressFinalize(this);
    }
}