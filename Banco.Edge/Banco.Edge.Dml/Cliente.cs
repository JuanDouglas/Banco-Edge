namespace Banco.Edge.Dml;
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string CpfOuCnpj
    {
        get => _cpfOuCnpj ?? string.Empty;
        set => _cpfOuCnpj = new string(value
                .Where(ch => char.IsDigit(ch))
                .Take(14)
                .ToArray());
    }
    private string? _cpfOuCnpj;
    public List<Conta>? Contas { get; private set; }

    public Cliente(int id, string nome, string telefone, string email, string cpfOuCnpj)
    {
        Id = id;
        Telefone = telefone ?? throw new ArgumentNullException(nameof(telefone));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        CpfOuCnpj = cpfOuCnpj ?? throw new ArgumentNullException(nameof(cpfOuCnpj));
    }

    private Cliente(int id, string name, string telefone, string email, string cpfOuCnpj, List<Conta>? contas)
        : this(id, name, telefone, email, cpfOuCnpj)
    {
        Contas = contas;
    }
}