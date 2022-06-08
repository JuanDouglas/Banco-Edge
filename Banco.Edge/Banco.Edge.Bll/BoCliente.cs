using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using Banco.Edge.Dal.Clientes;
using Banco.Edge.Bll.Exceptions;
using Banco.Edge.Bll.Base;
using Banco.Edge.Dal.Contas;
using static Banco.Edge.Dal.DaoBase;

namespace Banco.Edge.Bll;

public class BoCliente : BoBase
{
    public Cliente Cliente { get; set; }
    private protected DaoCliente DaoCliente { get; set; }
    private protected DaoConta DaoConta { get; set; }
    private protected static DaoCliente daoCliente
    {
        get
        {
            if (_daoCliente == null)
            {

                _daoCliente = new();
                _daoCliente.QueryExecuted += (object? sender, QueryEndEventArgs args) => { };
            }


            return _daoCliente;
        }
    }
    private static DaoCliente? _daoCliente;
    public BoCliente(Cliente cliente) : base()
    {
        Cliente = cliente;
        DaoCliente = new();
        DaoConta = new();
    }

    public async Task CriarContaAsync(TipoConta tipo)
    {
        Conta[]? contas = await DaoConta.BuscarContasAsync(Cliente.Id, tipo: tipo);

        // Um mesmo cliente não pode ter duas contas de mesmo tipo
        if (contas != null &&
            contas.Length > 0)
            throw new Exception();

        await DaoConta.CriarConta(tipo, Cliente.Id);
    }

    public async Task ObterContasAsync()
        => Cliente.Contas = await DaoConta.BuscarContasAsync(Cliente.Id);


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
        Cliente? busca = await daoCliente.ExisteAsync(cliente.Email, cliente.CpfOuCnpj);

        if (busca != null)
            throw new EmUsoException(busca.Email == cliente.Email ? nameof(cliente.Email) : nameof(cliente.CpfOuCnpj));

        int id = await daoCliente.InserirCliente(cliente);

        return id;
    }

    /// <summary>
    /// Busca um cliente pelo seu e-mail
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static async Task<Cliente?> BuscarAsync(string email, bool privado = true)
    {
        Cliente? cliente = await daoCliente.ExisteAsync(email, null, privado);

        return cliente;
    }

    public static async Task<Cliente[]?> ObterClientes(int idIncial, int maximo = 200)
        => await daoCliente.ListarClientes(idIncial, maximo);

    #endregion

    public override void Dispose()
    {
        DaoCliente.Dispose();
        DaoConta.Dispose();
        GC.Collect();
        GC.SuppressFinalize(this);
    }
}