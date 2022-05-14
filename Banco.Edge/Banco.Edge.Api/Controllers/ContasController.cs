using Banco.Edge.Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Banco.Edge.Api.Controllers;
public class ContasController : ApiController
{ 

    [HttpPost]
    [Route("Transferir")]
    public async Task<IActionResult> TransferirAsync()
    {
        throw new NotImplementedException();
    }
}
