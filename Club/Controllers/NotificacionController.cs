// Controllers/NotificacionController.cs
using Microsoft.AspNetCore.Mvc;
using Club.Data;
using System.Linq;

namespace Club.Controllers
{
    public class NotificacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener notificaciones del usuario
        [HttpGet]
        public IActionResult ObtenerNotificaciones()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
                return Json(new { success = false, message = "Usuario no autenticado." });

            int id = int.Parse(usuarioId);

            var notificaciones = _context.Notificaciones
                .Where(n => n.UsuarioId == id && !n.Leido)
                .OrderByDescending(n => n.Fecha)
                .Select(n => new { n.Mensaje, n.Fecha, n.NotificacionId })
                .ToList();

            return Json(new { success = true, notificaciones });
        }

        // Marcar una notificación como leída
        [HttpPost]
        public IActionResult MarcarComoLeida(int id)
        {
            var notificacion = _context.Notificaciones.FirstOrDefault(n => n.NotificacionId == id);
            if (notificacion == null)
                return Json(new { success = false, message = "Notificación no encontrada." });

            notificacion.Leido = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
