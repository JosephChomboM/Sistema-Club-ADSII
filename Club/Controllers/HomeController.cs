using Microsoft.AspNetCore.Mvc;

namespace Club.Controllers
{
    public class HomeController : Controller
    {
        // Página principal
        public IActionResult Index()
        {
            return View();
        }

        // Página de clubes
        public IActionResult Clubes()
        {
            return View();
        }

        // Página "About"
        public IActionResult About()
        {
            return View();
        }

        // Página de eventos
        public IActionResult Events()
        {
            return View();
        }

        // Página de contacto
        public IActionResult Contact()
        {
            return View();
        }
    }
}
