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
    public DaoLogin DaoLogin { get; set; }

    public BoLogin()
    {
        DaoLogin = new();
    }
    public Login LoginAsync(Cliente cliente, string senha, IPAddress adress)
    {
        if (!BCrypt.Net.BCrypt.Verify(senha, cliente.Senha))
            throw new SenhaInvalidaException();

        Login login = new(GerarToken(96), adress.GetAddressBytes(), cliente.Id);

        DaoLogin.InserirLogin(login);

        return login;
    }

    public static async Task<AuthenticationMidddleware.AuthenticationResult> GetAuthenticationAsync(HttpContext ctx)
    {
        bool isValid = false;
        bool isAuthenticated = false;

        dynamic header = ctx.Request.Headers["Authorization"].ToString();
        header = header.Split(' ');

        if ((int)header.Length < 2)
            return new(isValid, isAuthenticated);

        header = header[1].Split('.');

        if ((int)header.Length < 2)
            return new(isValid, isAuthenticated);

        if (string.IsNullOrEmpty((string)header))
            return new(isValid, isAuthenticated);

        DaoLogin dao = new();

        await dao.BuscarLoginAsync((string)header[0], (string)header[1]);
        throw new NotImplementedException();
    }
}