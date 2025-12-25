using Microsoft.AspNetCore.Mvc;

namespace P2WebMVC.Controllers
{
    public class StaticController : Controller
    {
        [HttpGet]
        public IActionResult Career()
        {
            return View();
        }
    }
}
