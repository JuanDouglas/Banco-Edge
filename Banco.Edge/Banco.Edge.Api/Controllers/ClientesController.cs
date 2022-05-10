using Microsoft.AspNetCore.Mvc;

namespace Banco.Edge.Api.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
