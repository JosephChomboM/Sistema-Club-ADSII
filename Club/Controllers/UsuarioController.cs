using Microsoft.AspNetCore.Mvc;
using Club.Data;
using Club.Models;
using BCrypt.Net; // Necesario para verificar las contraseñas

namespace Club.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar la vista de inicio de sesión
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Procesar el inicio de sesión
        [HttpPost]
        public IActionResult Login(string email, string contrasena)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contrasena))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar el correo y la contraseña.");
                return View();
            }

            // Buscar el usuario en la base de datos por email
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "El usuario no existe.");
                return View();
            }

            // Validar la contraseña
            if (!BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
            {
                ModelState.AddModelError(string.Empty, "La contraseña es incorrecta.");
                return View();
            }

            // Simular el inicio de sesión exitoso (en sistemas reales, usa cookies o autenticación)
            TempData["Mensaje"] = $"¡Bienvenido, {usuario.Nombre}!";
            return RedirectToAction("InicioExitoso");
        }

        // Mostrar mensaje de inicio exitoso
        public IActionResult InicioExitoso()
        {
            return View();
        }

        // Métodos de registro ya existentes (los dejamos tal como están)
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                ModelState.AddModelError("Email", "El correo ya está registrado.");
                return View(usuario);
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("RegistroExitoso");
        }

        public IActionResult RegistroExitoso()
        {
            return View();
        }
    }
}
