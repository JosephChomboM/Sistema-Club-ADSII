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


        // Admin Dashboard (Panel)
        public IActionResult Panel()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("LoginAdmin");
            }

            return View();
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
            // Ensure only logged-in admins can access this
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("LoginAdmin");
            }

            return View();
        }

        [HttpPost]
        public IActionResult CrearAdmin(Admin admin)
        {
            // Ensure only logged-in admins can access this
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("LoginAdmin");
            }

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
