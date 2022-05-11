using Nexus.Tools.Validations.Attributes;

namespace Banco.Edge.Api.Models;
public class Cliente
{
    public int Id { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nome { get; set; }
    [Required]
    [EmailAddress]
    [StringLength(500, MinimumLength = 7)]
    public string Email { get; set; }
    [Required]
    [CpfOrCnpj]
    public string CpfOuCnpj { get; set; }

    public Dml.Cliente ToDml()
        => new(Id, Nome, Email, CpfOuCnpj);
}