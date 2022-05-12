using Banco.Edge.Api.Controllers.Base;
using Banco.Edge.Api.Models;
using Banco.Edge.Bll;
using Banco.Edge.Bll.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Banco.Edge.Api.Controllers;
public class ClientesController : ApiController
{
    [HttpPut]
    [Route("Cadastro")]
    public async Task<IActionResult> CadastroAsync(Cliente cliente)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            cliente.Id = await BoCliente.CadastroAsync(cliente.ToDml());
        }
        catch (InUseException ex)
        {
            ModelState.AddModelError(ex.Field, ex.Message);
            return BadRequest(ModelState);
        }

        return Ok(cliente);
    }

    [HttpDelete]
    [Route("Delete")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}