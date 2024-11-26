using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Club.Data;
using Club.Models;

namespace Club.Controllers
{
    public class NotificacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificacionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult UsuariosParaMensajes()
        {
            var usuarios = _context.Usuarios.ToList();
            return View(usuarios); // Implicitly look for "Views/Notificacion/UsuariosParaMensajes.cshtml"
        }

        public IActionResult CrearMensaje(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            var notificacion = new Notificacion
            {
                UsuarioId = usuario.UsuarioId,
                Usuario = usuario
            };

            return View(notificacion); // Implicitly look for "Views/Notificacion/CrearMensaje.cshtml"
        }

        public IActionResult NotificacionesUsuario(int id)
        {
            var notificaciones = _context.Notificaciones
                .Include(n => n.Usuario)
                .Where(n => n.UsuarioId == id)
                .OrderByDescending(n => n.Fecha)
                .ToList();

            if (!notificaciones.Any())
            {
                TempData["Mensaje"] = "No hay notificaciones para este usuario.";
                return RedirectToAction("UsuariosParaMensajes");
            }

            return View(notificaciones); // Implicitly look for "Views/Notificacion/NotificacionesUsuario.cshtml"
        }
        [HttpGet]
        public IActionResult ObtenerNotificaciones()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Json(new { success = false, message = "Usuario no logueado." });
            }

            int userId = int.Parse(usuarioId);

            var notificaciones = _context.Notificaciones
                .Where(n => n.UsuarioId == userId && !n.Leido)
                .OrderByDescending(n => n.Fecha)
                .Select(n => new
                {
                    n.NotificacionId,       // ID de la notificación
                    n.Mensaje,              // El mensaje de la notificación
                    Fecha = n.Fecha.ToString("dd/MM/yyyy HH:mm") // Formato legible
                })
                .ToList();

            return Json(new { success = true, notificaciones });
        }




    }
}
