using System.Net;

namespace Banco.Edge.Api.Models.Result;
public class Login
{
    public string Token { get; set; }
    public string IP { get; set; }
    public DateTime Data { get; set; }
    public Login(Dml.Login login)
    {
        Token = login.Token;
        Data = login.Data;
        IP = new IPAddress(login.IP).ToString();
    }
}