﻿using Banco.Edge.Bll.Dml.Enums;

namespace Banco.Edge.Bll.Dml;

public class Conta 
{
    public int Id { get; set; }
    public decimal Saldo { get; set; }
    public TipoConta Tipo { get; set; }
    public DateTime Criacao { get; set; }
}