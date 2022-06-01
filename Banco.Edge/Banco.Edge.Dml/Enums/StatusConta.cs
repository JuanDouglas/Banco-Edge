using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Edge.Dml.Enums;
public enum StatusConta : byte
{
    Ativa = 0,
    Inativa = 25,
    Bloqueada = 50,
    Congelada = 100,
    Desativada = 200
}
