using Nexus.Tools.Validations.Attributes;

namespace Banco.Edge.Api.Models;
public class Cliente
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nome { get; set; }

    [Required]
    [Password]
    [StringLength(20, MinimumLength = 3)]
    public string Senha { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(500, MinimumLength = 7)]
    public string Email { get; set; }

    [Phone]
    [Required]
    [StringLength(15, MinimumLength = 8)]
    public string Telefone { get; set; }

    [Required]
    [CpfOrCnpj]
    public string CpfOuCnpj { get; set; }

    public string? Chave { get; internal set; }

    public Dml.Cliente ToDml()
        => new(Id, Nome, Telefone, Email, CpfOuCnpj, Senha, Chave ?? string.Empty, false);

}