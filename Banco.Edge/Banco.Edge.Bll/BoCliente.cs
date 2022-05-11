using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using Banco.Edge.Dal.Clientes;

namespace Banco.Edge.Bll;

public class BoCliente
{
    public Cliente Cliente { get; set; }
    public BoCliente(Cliente cliente)
    {
        Cliente = cliente;
    }

    public void CriarConta(TipoConta tipo)
    {
    }

    public static async Task<int> CadastroAsync(Cliente cliente)
    {
        DaoCliente dao = new();

        Cliente? busca = await dao.ExisteAsync(cliente.Email, cliente.CpfOuCnpj);

        if (busca != null)
            throw new Exception(busca.Email == cliente.Email ? "Email em uso" : "Cpf ou CNpj esta em Uso");

        int id = await dao.InserirCliente(cliente.Nome, cliente.Email, cliente.CpfOuCnpj);

        return id;
    }
}