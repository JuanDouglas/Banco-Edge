using Banco.Edge.Bll.Base;
using Banco.Edge.Bll.Exceptions;
using Banco.Edge.Dal.Logins;
using Banco.Edge.Dml;
using Microsoft.AspNetCore.Http;
using Nexus.Tools.Validations.Middlewares.Authentication;
using System.Net;

namespace Banco.Edge.Bll;
public class BoLogin : BoBase
{
    public async Task<Login> LoginAsync(Cliente cliente, string senha, IPAddress adress)
    {
        if (!BCrypt.Net.BCrypt.Verify(senha, cliente.Senha))
            throw new SenhaInvalidaException();

        Login login = new(GerarToken(96), adress.GetAddressBytes(), cliente.Id);
        DaoLogin dao = new();

        await dao.InsertLoginAsync(login);

        return login;
    }

    public static async Task<AuthenticationMidddleware.AuthenticationResult> GetAuthenticationAsync(HttpContext ctx)
    {
        throw new NotImplementedException();
    }
}