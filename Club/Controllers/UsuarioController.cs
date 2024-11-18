// Controllers/UsuarioController.cs
using Microsoft.AspNetCore.Mvc;
using Club.Data;
using Club.Models;
using BCrypt.Net; // Instala BCrypt.Net-Next

namespace Club.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar la vista de registro
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        // Procesar el registro de usuario
        [HttpPost]
        public IActionResult Registrar(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            // Validar si el correo ya existe
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                ModelState.AddModelError("Email", "El correo ya está registrado.");
                return View(usuario);
            }

            // Hashear la contraseña antes de guardar
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            // Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("RegistroExitoso");
        }

        // Mostrar mensaje de éxito
        public IActionResult RegistroExitoso()
        {
            return View();
        }
    }
}
