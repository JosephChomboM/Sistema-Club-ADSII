using Microsoft.AspNetCore.Mvc;
using Club.Data;
using Club.Models;
using BCrypt.Net;

namespace Club.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Login Page
        [HttpGet]
        [Route("Admin/LoginAdmin")] // Explicitly set the route
        public IActionResult LoginAdmin()
        {
            return View("LoginAdmin");
        }

        [HttpPost]
        [Route("Admin/LoginAdmin")] // Explicitly set the route
        public IActionResult LoginAdmin(string email, string contrasena)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contrasena))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar el correo y la contraseña.");
                return View("LoginAdmin");
            }

            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);
            if (admin == null)
            {
                ModelState.AddModelError(string.Empty, "El administrador no existe.");
                return View("LoginAdmin");
            }

            if (!BCrypt.Net.BCrypt.Verify(contrasena, admin.Contrasena))
            {
                ModelState.AddModelError(string.Empty, "La contraseña es incorrecta.");
                return View("LoginAdmin");
            }

            // Save admin session details
            HttpContext.Session.SetString("AdminEmail", admin.Email);
            HttpContext.Session.SetString("AdminNombre", admin.Nombre);
            HttpContext.Session.SetString("AdminId", admin.AdminId.ToString());

            return RedirectToAction("Panel");
        }

        public IActionResult Panel()
        {
            var clubes = _context.Lugares.ToList();
            return View(clubes);
        }

        // GET: Create a new club
        [HttpGet]
        public IActionResult CrearClub()
        {
            return View();
        }

        // POST: Create a new club
        [HttpPost]
        public IActionResult CrearClub(Lugar lugar)
        {
            if (!ModelState.IsValid)
            {
                return View(lugar);
            }

            _context.Lugares.Add(lugar);
            _context.SaveChanges();

            TempData["Mensaje"] = "Club creado exitosamente.";
            return RedirectToAction("Panel");
        }

        // GET: Edit a club
        [HttpGet]
        public IActionResult EditarClub(int id)
        {
            var lugar = _context.Lugares.FirstOrDefault(l => l.LugarId == id);
            if (lugar == null)
            {
                return NotFound();
            }

            return View(lugar);
        }

        // POST: Edit a club
        [HttpPost]
        public IActionResult EditarClub(Lugar lugar)
        {
            if (!ModelState.IsValid)
            {
                return View(lugar);
            }

            _context.Lugares.Update(lugar);
            _context.SaveChanges();

            TempData["Mensaje"] = "Club actualizado exitosamente.";
            return RedirectToAction("Panel");
        }

        // GET: Delete a club
        [HttpGet]
        public IActionResult EliminarClub(int id)
        {
            var lugar = _context.Lugares.FirstOrDefault(l => l.LugarId == id);
            if (lugar == null)
            {
                return NotFound();
            }

            return View(lugar);
        }

        // POST: Confirm deletion
        [HttpPost]
        public IActionResult ConfirmarEliminarClub(int id)
        {
            var lugar = _context.Lugares.FirstOrDefault(l => l.LugarId == id);
            if (lugar == null)
            {
                return NotFound();
            }

            _context.Lugares.Remove(lugar);
            _context.SaveChanges();

            TempData["Mensaje"] = "Club eliminado exitosamente.";
            return RedirectToAction("Panel");
        }

        // Logout Admin
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginAdmin");
        }

        [HttpGet]
        public IActionResult CrearAdmin()
        {
            // Allow access without session validation for the first admin creation
            if (!_context.Admins.Any())
            {
                return View(); // Allow access if no admin exists
            }

            // Require session validation if admins exist
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("LoginAdmin");
            }

            return View();
        }

        [HttpPost]
        public IActionResult CrearAdmin(Admin admin)
        {
            // Allow access without session validation for the first admin creation
            if (!_context.Admins.Any())
            {
                return SaveAdmin(admin); // Directly save if no admin exists
            }

            // Require session validation if admins exist
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("LoginAdmin");
            }

            return SaveAdmin(admin);
        }

        private IActionResult SaveAdmin(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return View(admin);
            }

            // Check if the email already exists in the admin database
            if (_context.Admins.Any(a => a.Email == admin.Email))
            {
                ModelState.AddModelError("Email", "El correo ya está registrado como administrador.");
                return View(admin);
            }

            // Hash the password before saving
            admin.Contrasena = BCrypt.Net.BCrypt.HashPassword(admin.Contrasena);

            // Save the new admin to the database
            _context.Admins.Add(admin);
            _context.SaveChanges();

            TempData["Mensaje"] = "Administrador creado exitosamente.";
            return RedirectToAction("Panel");
        }
    }
}
