using Banco.Edge.Api.Controllers.Base;
using Banco.Edge.Bll;
using Banco.Edge.Bll.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Nexus.Tools.Validations.Middlewares.Authentication.Attributes;
using System.Net;

namespace Banco.Edge.Api.Controllers;

[AllowAnonymous]
public class AutenticacaoController : ApiController
{
    public static BoLogin BoLogin { get; set; } = new();

    public AutenticacaoController()
    {

    }

    [HttpGet]
    [Route("Login")]
    public async Task<IActionResult> LoginAsync(string user, string senha)
    {
        Dml.Cliente? cliente = await BoCliente.BuscarAsync(user, false);

        if (cliente == null)
            return Unauthorized();

        try
        {
            IPAddress address = HttpContext.Connection.RemoteIpAddress ??
                new IPAddress(new byte[] { 127, 0, 0, 1 });

            Dml.Login login = await BoLogin.LoginAsync(cliente, senha, address);

            return Ok(new Models.Result.Login(login));
        }
        catch (SenhaInvalidaException)
        {
            return Unauthorized();
        }
    }
}