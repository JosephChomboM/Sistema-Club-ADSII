using Club.Data;
using Microsoft.AspNetCore.Mvc;

namespace Club.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Página principal protegida
        public IActionResult Dashboard()
        {
            // Validar si el usuario está logueado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            ViewData["UsuarioNombre"] = HttpContext.Session.GetString("UsuarioNombre");
            return View();
        }
        // Catálogo de lugares
        public IActionResult CatalogoLugares()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            var lugares = _context.Lugares.ToList();
            return View(lugares);
        }

        // Detalle de lugar
        public IActionResult DetalleLugar(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            var lugar = _context.Lugares
                .Where(l => l.LugarId == id)
                .Select(l => new
                {
                    l.Nombre,
                    l.Direccion,
                    Espacios = l.Espacios.Select(e => new { e.EspacioId, e.Nombre}) // Incluye EspacioId
                })
                .FirstOrDefault();

            if (lugar == null)
            {
                return NotFound();
            }

            ViewData["LugarNombre"] = lugar.Nombre;
            ViewData["LugarDireccion"] = lugar.Direccion;

            return View(lugar.Espacios);
        }

        // Página principal
        public IActionResult Index()
        {
            return View();
        }

        // Lista de clubes
        public IActionResult Clubes()
        {
            var clubes = _context.Lugares.ToList();
            return View(clubes);
        }

        // Detalle de un club
        public IActionResult DetalleClub(int id)
        {
            var club = _context.Lugares.FirstOrDefault(l => l.LugarId == id);
            if (club == null)
            {
                return NotFound();
            }
            return View(club);
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
