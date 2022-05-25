using Banco.Edge.Dml.Enums;

namespace Banco.Edge.Dml;

public class Transacao
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public TipoTransacao Tipo { get; set; }
    public int ParaId { get; set; }

    public Transacao(int id, decimal valor, DateTime data, TipoTransacao tipo, int paraId)
    {
        Id = id;
        Valor = valor;
        Data = data;
        Tipo = tipo;
        ParaId = paraId;
    }
}