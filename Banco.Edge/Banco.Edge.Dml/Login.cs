namespace Banco.Edge.Dml;
public class Login
{
    public string Token { get; set; }
    public byte[] IP { get; set; }
    public int ClienteId { get; set; }
    public DateTime Data { get; set; }

    public Login(string token, byte[] ip, int clienteId)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        IP = ip ?? throw new ArgumentNullException(nameof(ip));
        ClienteId = clienteId;
        Data = DateTime.Now;
    }

    public Login(string token, byte[] ip, int clienteId, DateTime data)
        : this(token, ip, clienteId)
    {
        Data = data;
    }
}