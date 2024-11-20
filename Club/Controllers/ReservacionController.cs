using Club.Data;
using Club.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Club.Controllers
{
    public class ReservacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ConsultarDisponibilidad(int espacioId, DateTime? fecha)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            var espacio = _context.Espacios.FirstOrDefault(e => e.EspacioId == espacioId);
            if (espacio == null)
            {
                return NotFound("El espacio no existe.");
            }

            if (!fecha.HasValue)
            {
                ViewData["EspacioId"] = espacioId;
                ViewData["EspacioNombre"] = espacio.Nombre;
                return View(null); // Retorna solo el formulario
            }

            // Consultar reservaciones en la fecha seleccionada
            var reservaciones = _context.Reservaciones
                .Where(r => r.EspacioId == espacioId && r.FechaInicio.Date == fecha.Value.Date)
                .Select(r => new
                {
                    HoraInicio = r.FechaInicio.TimeOfDay,
                    HoraFin = r.FechaFin.TimeOfDay
                })
                .ToList();

            var inicioDia = new TimeSpan(8, 0, 0);
            var finDia = new TimeSpan(20, 0, 0);
            var intervalo = new TimeSpan(1, 0, 0);

            var horasDisponibles = new List<(DateTime Inicio, DateTime Fin)>();

            for (var hora = inicioDia; hora < finDia; hora += intervalo)
            {
                var siguienteHora = hora + intervalo;

                // Verificar disponibilidad
                var ocupado = reservaciones.Any(r =>
                    (hora >= r.HoraInicio && hora < r.HoraFin) ||
                    (siguienteHora > r.HoraInicio && siguienteHora <= r.HoraFin) ||
                    (hora <= r.HoraInicio && siguienteHora >= r.HoraFin));

                if (!ocupado)
                {
                    horasDisponibles.Add((fecha.Value.Date + hora, fecha.Value.Date + siguienteHora));
                }
            }

            ViewData["EspacioId"] = espacioId;
            ViewData["EspacioNombre"] = espacio.Nombre;
            ViewData["Fecha"] = fecha.Value.ToString("yyyy-MM-dd");

            return View(horasDisponibles);
        }




        // Mostrar formulario de creación
        [HttpGet]
        public IActionResult Crear(int espacioId, DateTime fechaInicio, DateTime fechaFin)
        {
            var espacio = _context.Espacios.FirstOrDefault(e => e.EspacioId == espacioId);
            if (espacio == null)
            {
                return NotFound("El espacio no existe.");
            }

            ViewData["EspacioNombre"] = espacio.Nombre;
            ViewData["EspacioId"] = espacioId;
            ViewData["FechaInicio"] = fechaInicio;
            ViewData["FechaFin"] = fechaFin;

            return View();
        }



        // Procesar la reservación
        [HttpPost]
        public IActionResult Crear(int espacioId, DateTime fechaInicio, DateTime fechaFin, string detalles)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (fechaInicio >= fechaFin)
            {
                ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
                return View();
            }

            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            var reservacion = new Reservacion
            {
                UsuarioId = usuarioId,
                EspacioId = espacioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Detalles = detalles
            };

            _context.Reservaciones.Add(reservacion);
            _context.SaveChanges();

            return RedirectToAction("Confirmacion");
        }




        // Confirmación
        public IActionResult Confirmacion()
        {
            return View();
        }

        public IActionResult ConsultarReservas()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));
            var reservaciones = _context.Reservaciones
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Espacio)
                .ToList();

            return View(reservaciones);
        }


    }
}
