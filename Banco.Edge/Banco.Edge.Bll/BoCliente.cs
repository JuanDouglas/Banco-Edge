using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using Banco.Edge.Dal.Clientes;
using Banco.Edge.Bll.Exceptions;
using System.Data.SqlClient;
using Banco.Edge.Bll.Base;

namespace Banco.Edge.Bll;

public class BoCliente : BoBase
{
    public Cliente Cliente { get; set; }
    private protected DaoCliente DaoCliente { get; set; }
    public BoCliente(Cliente cliente) : base()
    {
        Cliente = cliente;
        DaoCliente = new();
    }

    public void CriarConta(TipoConta tipo)
    {

    }

    public static async Task AtualizarAsync(Cliente cliente)
    {

    }

    public static async Task<int> CadastroAsync(Cliente cliente)
    {
        using DaoCliente dao = new();

        Cliente? busca = await dao.ExisteAsync(cliente.Email, cliente.CpfOuCnpj);

        if (busca != null)
            throw new EmUsoException(busca.Email == cliente.Email ? nameof(cliente.Email) : nameof(cliente.CpfOuCnpj));

        int id = await dao.InserirCliente(cliente);

        return id;
    }

    public static async Task<Cliente?> BuscarAsync(string email, bool ocultarSensiveis = true)
    {
        using DaoCliente dao = new();

        Cliente? cliente = await dao.ExisteAsync(email, null, ocultarSensiveis);

        return cliente;
    }
}