using Banco.Edge.Api.Controllers.Base;
using Banco.Edge.Api.Models;
using Banco.Edge.Bll;
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
        catch (Exception)
        {

            throw;
        }

        return Ok(cliente);
    }
}