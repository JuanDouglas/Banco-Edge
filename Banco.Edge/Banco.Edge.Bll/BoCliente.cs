using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using Banco.Edge.Dal.Clientes;
using Banco.Edge.Bll.Exceptions;
using Banco.Edge.Bll.Base;
using Banco.Edge.Dal.Contas;

namespace Banco.Edge.Bll;

public class BoCliente : BoBase
{
    public Cliente Cliente { get; set; }
    private protected DaoCliente DaoCliente { get; set; }
    private protected DaoConta DaoConta { get; set; }
    public BoCliente(Cliente cliente) : base()
    {
        Cliente = cliente;
        DaoCliente = new();
        DaoConta = new();
    }

    public async Task CriarContaAsync(TipoConta tipo)
    {
        Conta? conta = await DaoConta.BuscarContaAsync(Cliente.Id, tipo);

        if (conta == null)
            throw new Exception();

        await DaoConta.CriarConta(tipo, Cliente.Id);
    }

    public async Task ObterContasAsync() 
    {
        await DaoConta.ObterContasAsync(Cliente.Id);
    }

    #region CRUD
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cliente"></param>
    /// <returns></returns>
    public async Task AtualizarAsync(Cliente cliente)
    {

    }

    /// <summary>
    /// Exclui um cliente é todos seus registros relacionados.
    /// </summary>
    /// <param name="senha">Senha do cliente.</param>
    /// <returns></returns>
    /// <exception cref="SenhaInvalidaException"></exception>
    public async Task ExcluirAsync(string senha)
    {
        if (!BCrypt.Net.BCrypt.Verify(senha, Cliente.Senha))
            throw new SenhaInvalidaException();

        await DaoCliente.ExcluirAsync(Cliente.Id);
    }

    /// <summary>
    /// Cadastro um novo cliente no banco.
    /// </summary>
    /// <param name="cliente"></param>
    /// <returns></returns>
    /// <exception cref="EmUsoException"></exception>
    public static async Task<int> CadastroAsync(Cliente cliente)
    {
        using DaoCliente dao = new();

        Cliente? busca = await dao.ExisteAsync(cliente.Email, cliente.CpfOuCnpj);

        if (busca != null)
            throw new EmUsoException(busca.Email == cliente.Email ? nameof(cliente.Email) : nameof(cliente.CpfOuCnpj));

        int id = await dao.InserirCliente(cliente);

        return id;
    }

    /// <summary>
    /// Busca um cliente pelo seu e-mail
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static async Task<Cliente?> BuscarAsync(string email, bool privado = true)
    {
        using DaoCliente dao = new();

        Cliente? cliente = await dao.ExisteAsync(email, null, privado);

        return cliente;
    }
    #endregion
}