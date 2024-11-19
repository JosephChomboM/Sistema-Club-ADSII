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

        // Consultar disponibilidad de horarios
        public IActionResult ConsultarDisponibilidad(int espacioId, DateTime fecha)
        {
            Console.WriteLine($"EspacioId recibido: {espacioId}");
            Console.WriteLine($"Fecha recibida: {fecha}");

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            var espacio = _context.Espacios.FirstOrDefault(e => e.EspacioId == espacioId);
            if (espacio == null)
            {
                Console.WriteLine("Error: El espacio no existe.");
                return NotFound("El espacio no existe.");
            }

            // Horarios reservados en la fecha específica
            var reservaciones = _context.Reservaciones
                .Where(r => r.EspacioId == espacioId && r.FechaInicio.Date == fecha.Date)
                .Select(r => new
                {
                    HoraInicio = r.FechaInicio.TimeOfDay, // Asigna nombres únicos
                    HoraFin = r.FechaFin.TimeOfDay        // Asigna nombres únicos
                })
                .ToList();

            // Generar horarios disponibles (8 AM a 8 PM)
            var inicioDia = new TimeSpan(8, 0, 0);
            var finDia = new TimeSpan(20, 0, 0);
            var intervalo = new TimeSpan(1, 0, 0);

            var horasDisponibles = new List<(TimeSpan Inicio, TimeSpan Fin)>();

            for (var hora = inicioDia; hora < finDia; hora += intervalo)
            {
                var siguienteHora = hora + intervalo;

                // Verificar si el horario está ocupado
                var ocupado = reservaciones.Any(r =>
                    (hora >= r.HoraInicio && hora < r.HoraFin) || // Inicio en horario reservado
                    (siguienteHora > r.HoraInicio && siguienteHora <= r.HoraFin) || // Fin en horario reservado
                    (hora <= r.HoraInicio && siguienteHora >= r.HoraFin)); // Contiene el horario reservado

                if (!ocupado)
                {
                    horasDisponibles.Add((hora, siguienteHora));
                }
            }

            ViewData["EspacioNombre"] = espacio.Nombre;
            ViewData["EspacioId"] = espacioId;
            ViewData["Fecha"] = fecha.ToString("yyyy-MM-dd");
            return View(horasDisponibles);
        }


        // Mostrar formulario de creación
        [HttpGet]
        public IActionResult Crear(int espacioId, TimeSpan fechaInicio, TimeSpan fechaFin, DateTime fecha)
        {
            var espacio = _context.Espacios.FirstOrDefault(e => e.EspacioId == espacioId);
            if (espacio == null)
            {
                return NotFound("El espacio no existe.");
            }

            ViewData["EspacioNombre"] = espacio.Nombre;
            ViewData["EspacioId"] = espacioId;
            ViewData["FechaInicio"] = fecha.Date + fechaInicio; // Combina fecha y hora
            ViewData["FechaFin"] = fecha.Date + fechaFin;

            return View();
        }


        // Procesar la reservación
        [HttpPost]
        public IActionResult Crear(int espacioId, DateTime fechaInicio, DateTime fechaFin, string detalles)
        {
            // Agregar mensajes de depuración
            Console.WriteLine($"EspacioId recibido: {espacioId}");
            Console.WriteLine($"FechaInicio: {fechaInicio}, FechaFin: {fechaFin}");
            Console.WriteLine($"Detalles: {detalles}");

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            // Verificar si el EspacioId existe en la base de datos
            var espacio = _context.Espacios.FirstOrDefault(e => e.EspacioId == espacioId);
            if (espacio == null)
            {
                ModelState.AddModelError("", "El espacio seleccionado no existe.");
                return View();
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

        // Ver reservas

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Login", "Usuario");
            }

            // Obtener todas las reservas del usuario logueado
            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));
            var reservas = _context.Reservaciones
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Espacio) // Traer datos del espacio relacionado
                .ThenInclude(e => e.Lugar) // Traer datos del lugar relacionado
                .ToList();

            return View(reservas);
        }

    }
}
