﻿using Banco.Edge.Api.Controllers.Base;
using Banco.Edge.Api.Models;
using Banco.Edge.Bll;
using Banco.Edge.Bll.Base;
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
            cliente.Chave = BoBase.GerarToken(96);
            cliente.Id = await BoCliente.CadastroAsync(cliente.ToDml());
            cliente.Senha = string.Empty;
            cliente.Chave = string.Empty;
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
    public async Task<IActionResult> AtualizarAsync(Cliente cliente, string? senha)
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