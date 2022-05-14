using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Edge.Dml;
public class Login
{
    public string Token { get; set; }
    public byte[] IP { get; set; }
    public int ClienteId { get; set; }

    public Login(string token, byte[] ip, int clienteId)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        IP = ip ?? throw new ArgumentNullException(nameof(ip));
        ClienteId = clienteId;
    }
}