using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using Banco.Edge.Dal.Clientes;
using Banco.Edge.Bll.Exceptions;
using System.Data.SqlClient;

namespace Banco.Edge.Bll;

public class BoCliente
{
    public Cliente Cliente { get; set; }
    public BoCliente(Cliente cliente) : base()
    {
        Cliente = cliente;
    }

    public void CriarConta(TipoConta tipo)
    {

    }

    public static async Task<int> CadastroAsync(Cliente cliente)
    {
        using DaoCliente dao = new();

        Cliente? busca = await dao.ExisteAsync(cliente.Email, cliente.CpfOuCnpj);

        if (busca != null)
            throw new InUseException(busca.Email == cliente.Email ? nameof(cliente.Email) : nameof(cliente.CpfOuCnpj));

        int id = await dao.InserirCliente(cliente);

        return id;
    }
}