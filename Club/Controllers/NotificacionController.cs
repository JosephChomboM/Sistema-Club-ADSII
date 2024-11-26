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

    }
}
