namespace Banco.Edge.Bll.Dml;
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string CpfOuCnpj { get; set; }
    public List<Conta>? Contas { get; private set; }

    public Cliente(int id, string nome, string cpfOuCnpj)
    {
        Id = id;
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        CpfOuCnpj = cpfOuCnpj ?? throw new ArgumentNullException(nameof(cpfOuCnpj));
    }

    internal Cliente(int id, string name, string cpfOuCnpj, List<Conta>? contas) : this(id, name, cpfOuCnpj)
    {
        Contas = contas;
    }
}