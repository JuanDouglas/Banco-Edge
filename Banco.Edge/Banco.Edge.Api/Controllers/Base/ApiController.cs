using Microsoft.AspNetCore.Mvc;
using Nexus.Tools.Validations.Middlewares.Authentication.Attributes;

namespace Banco.Edge.Api.Controllers.Base;

[ApiController]
[RequireAuthentication]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
}