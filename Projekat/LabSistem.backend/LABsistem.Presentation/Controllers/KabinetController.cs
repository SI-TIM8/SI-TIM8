using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    public class KabinetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
