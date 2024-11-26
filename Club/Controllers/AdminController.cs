using Microsoft.AspNetCore.Mvc;
using Club.Data;
using Club.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

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
        // GET: Crear un nuevo club
        [HttpGet]
        public IActionResult CrearClub()
        {
            // Devuelve un objeto vacío de Lugar para ser llenado en la vista
            var lugar = new Lugar
            {
                Espacios = new List<Espacio>() // Inicializamos la lista de espacios
            };
            return View(lugar);
        }

        [HttpPost]
        public IActionResult CrearClub(Lugar lugar, List<Espacio> espacios)
        {
            if (!ModelState.IsValid)
            {
                lugar.Espacios = espacios ?? new List<Espacio>();
                return View(lugar);
            }

            // Crear un nuevo lugar
            var nuevoLugar = new Lugar
            {
                Nombre = lugar.Nombre,
                Direccion = lugar.Direccion,
                Descripcion = lugar.Descripcion,
                LogoUrl = lugar.LogoUrl,
                Espacios = new List<Espacio>()
            };

            // Agregar espacios al lugar
            foreach (var espacio in espacios)
            {
                nuevoLugar.Espacios.Add(new Espacio
                {
                    Nombre = espacio.Nombre,
                    Precio = espacio.Precio
                });
            }

            _context.Lugares.Add(nuevoLugar);
            _context.SaveChanges();

            TempData["Mensaje"] = "Club creado exitosamente.";
            return RedirectToAction("Panel");
        }





        // GET: Edit Club and its Espacios
        [HttpGet]
        public IActionResult EditarClub(int id)
        {
            var lugar = _context.Lugares
                .Include(l => l.Espacios) // Incluimos los espacios asociados al club
                .FirstOrDefault(l => l.LugarId == id);

            if (lugar == null)
            {
                return NotFound();
            }

            return View(lugar);
        }

        // POST: Save Club and its Espacios
        [HttpPost]
        public IActionResult EditarClub(Lugar lugar, List<Espacio> espacios)
        {
            // Buscar el lugar existente con sus espacios
            var lugarExistente = _context.Lugares
                .Include(l => l.Espacios) // Incluye los espacios para poder trabajar con ellos
                .FirstOrDefault(l => l.LugarId == lugar.LugarId);

            if (lugarExistente == null)
            {
                return NotFound();
            }

            // Actualizar datos del lugar
            lugarExistente.Nombre = lugar.Nombre;
            lugarExistente.Direccion = lugar.Direccion;
            lugarExistente.Descripcion = lugar.Descripcion;
            lugarExistente.LogoUrl = lugar.LogoUrl;

            // Espacios existentes en la base de datos
            var espaciosExistentes = lugarExistente.Espacios.ToList();

            // Actualizar o agregar espacios
            foreach (var espacioActualizado in espacios)
            {
                var espacioExistente = espaciosExistentes
                    .FirstOrDefault(e => e.EspacioId == espacioActualizado.EspacioId);

                if (espacioExistente != null)
                {
                    // Actualizar espacio existente
                    espacioExistente.Nombre = espacioActualizado.Nombre;
                    espacioExistente.Precio = espacioActualizado.Precio;
                }
                else
                {
                    // Agregar nuevo espacio
                    lugarExistente.Espacios.Add(new Espacio
                    {
                        Nombre = espacioActualizado.Nombre,
                        Precio = espacioActualizado.Precio,
                        LugarId = lugar.LugarId
                    });
                }
            }

            // Eliminar espacios no enviados
            var idsEspaciosEnviados = espacios.Select(e => e.EspacioId).ToList();
            var espaciosParaEliminar = espaciosExistentes
                .Where(e => !idsEspaciosEnviados.Contains(e.EspacioId))
                .ToList();

            foreach (var espacioEliminado in espaciosParaEliminar)
            {
                _context.Espacios.Remove(espacioEliminado);
            }

            // Guardar cambios en la base de datos
            _context.SaveChanges();

            TempData["Mensaje"] = "Club y espacios actualizados correctamente.";
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
        // POST: Confirmar eliminación
        [HttpPost]
        [ValidateAntiForgeryToken] // Protección contra ataques CSRF
        public IActionResult ConfirmarEliminarClub(int id)
        {
            // Buscar el club a eliminar
            var lugar = _context.Lugares
                .Include(l => l.Espacios) // Incluye los espacios relacionados para eliminarlos también
                .FirstOrDefault(l => l.LugarId == id);

            if (lugar == null)
            {
                TempData["Error"] = "El club que intentas eliminar no existe.";
                return RedirectToAction("Panel");
            }

            // Eliminar los espacios relacionados primero (si existen)
            if (lugar.Espacios.Any())
            {
                _context.Espacios.RemoveRange(lugar.Espacios);
            }

            // Eliminar el club
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
