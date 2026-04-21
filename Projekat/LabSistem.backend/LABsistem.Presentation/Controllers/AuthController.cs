using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
