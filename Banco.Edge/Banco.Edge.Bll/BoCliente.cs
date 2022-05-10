using Banco.Edge.Dml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Edge.Bll
{
    public class BoCliente
    {
        public Cliente Cliente { get; set; }
        public BoCliente(Cliente cliente)
        {
            Cliente = cliente;
        }

        public static int Inserir()
        {
            throw new NotImplementedException();
        }
    }
}
