using Banco.Edge.Api.Controllers.Base;
using Banco.Edge.Bll;
using Banco.Edge.Bll.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Banco.Edge.Api.Controllers;
public class AutenticacaoController : ApiController
{
    public BoLogin BoLogin { get; set; }

    public AutenticacaoController()
    {
        BoLogin = new();
    }

    [HttpGet]
    [Route("Login")]
    public async Task<IActionResult> LoginAsync(string user, string senha)
    {
        Dml.Cliente? cliente = await BoCliente.BuscarAsync(user);

        if (cliente == null)
            return Unauthorized();

        try
        {
            IPAddress address = HttpContext.Connection.RemoteIpAddress ??
                new IPAddress(new byte[] { 127, 0, 0, 1 });

            var login = await BoLogin.LoginAsync(cliente, senha, address);
            return Ok(login);
        }
        catch (SenhaInvalidaException)
        {

            return Unauthorized();
        }
    }
}