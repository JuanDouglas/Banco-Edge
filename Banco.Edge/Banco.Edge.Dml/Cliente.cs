namespace Banco.Edge.Dml;
public class Cliente
{
    private const int workFactor = 11;
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha
    {
        get => _senha;
        set
        {
            _senha = BCrypt.Net.BCrypt.HashPassword(value, workFactor);
        }
    }
    private string _senha;
    public string Chave { get; set; }
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

    public Cliente(int id, string nome, string telefone, string email, string cpfOuCnpj, string senha, string chave, bool privado)
    {
        Id = id;
        Telefone = telefone ?? throw new ArgumentNullException(nameof(telefone));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        CpfOuCnpj = cpfOuCnpj ?? throw new ArgumentNullException(nameof(cpfOuCnpj));

        if (!privado)
        {
            Chave = chave ?? throw new ArgumentNullException(nameof(chave));
            _senha = senha ?? throw new ArgumentNullException(nameof(senha));
        }
        else
        {
            Chave = string.Empty;
            _senha = string.Empty;
        }
    }

    public Cliente(int id, string name, string telefone, string email, string cpfOuCnpj, string senha, string chave)
        : this(id, name, telefone, email, cpfOuCnpj, BCrypt.Net.BCrypt.HashPassword(senha), chave, false)
    {
    }
}