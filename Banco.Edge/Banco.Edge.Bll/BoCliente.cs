using Banco.Edge.Bll.Dml;
using Banco.Edge.Bll.Dml.Enums;

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

    public static int Cadastro(Cliente cliente)
    {

    }
}