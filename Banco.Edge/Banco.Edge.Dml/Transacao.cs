using Banco.Edge.Dml.Enums;

namespace Banco.Edge.Dml;

public class Transacao
{
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public TipoTransacao Tipo { get; set; }
}