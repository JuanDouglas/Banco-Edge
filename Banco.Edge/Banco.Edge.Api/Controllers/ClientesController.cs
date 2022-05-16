using Banco.Edge.Api.Controllers.Base;
using Banco.Edge.Api.Models;
using Banco.Edge.Bll;
using Banco.Edge.Bll.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Nexus.Tools.Validations.Middlewares.Authentication.Attributes;

namespace Banco.Edge.Api.Controllers;
public class ClientesController : ApiController
{
    [HttpPut]
    [AllowAnonymous]
    [Route("Cadastro")]
    public async Task<IActionResult> CadastroAsync(Cliente cliente)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            cliente.Id = await BoCliente.CadastroAsync(cliente.ToDml());
            cliente.Senha = string.Empty;
        }
        catch (EmUsoException ex)
        {
            ModelState.AddModelError(ex.Field, ex.Message);
            return BadRequest(ModelState);
        }

        return Ok(cliente);
    }

    [HttpPost]
    [Route("Atualizar")]
    public async Task<IActionResult> AtualizarAsync(Cliente cliente)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    [Route("Excluir")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}