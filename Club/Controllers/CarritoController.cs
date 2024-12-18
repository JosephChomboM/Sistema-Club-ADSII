﻿using Club.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Club.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarritoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar los elementos del carrito
        public IActionResult Index()
        {
            var carrito = HttpContext.Session.GetString("Carrito");
            var listaCarrito = string.IsNullOrEmpty(carrito)
                ? new List<dynamic>()
                : JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            return View(listaCarrito);
        }

        // Agregar un espacio al carrito
        public IActionResult Agregar(int espacioId, DateTime fechaInicio, DateTime fechaFin)
        {
            var espacio = _context.Espacios.FirstOrDefault(e => e.EspacioId == espacioId);
            if (espacio == null)
            {
                return NotFound("El espacio no existe.");
            }

            var carrito = HttpContext.Session.GetString("Carrito");
            var listaCarrito = string.IsNullOrEmpty(carrito)
                ? new List<dynamic>()
                : JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            // Verificar si ya existe una reserva para este espacio
            var itemExistente = listaCarrito.FirstOrDefault(item =>
                item.EspacioId == espacioId &&
                (
                    (DateTime)item.FechaFin == fechaInicio || // Nueva hora inicia justo después
                    (DateTime)item.FechaInicio == fechaFin    // Nueva hora termina justo antes
                )
            );

            if (itemExistente != null)
            {
                // Unificar la reserva existente con la nueva
                if ((DateTime)itemExistente.FechaFin == fechaInicio)
                {
                    itemExistente.FechaFin = fechaFin; // Extiende el fin de la reserva
                }
                else if ((DateTime)itemExistente.FechaInicio == fechaFin)
                {
                    itemExistente.FechaInicio = fechaInicio; // Extiende el inicio de la reserva
                }

                // Actualiza el precio
                itemExistente.Precio += espacio.Precio;
            }
            else
            {
                // Si no existe, agregar como una nueva entrada
                var itemCarrito = new
                {
                    EspacioId = espacio.EspacioId,
                    NombreEspacio = espacio.Nombre,
                    Precio = espacio.Precio,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };
                listaCarrito.Add(itemCarrito);
            }

            // Guardar el carrito actualizado en la sesión
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(listaCarrito));

            return RedirectToAction("Index");
        }





        // Eliminar un elemento del carrito
        [HttpPost]
        public IActionResult Eliminar(int espacioId, DateTime fechaInicio, DateTime fechaFin)
        {
            var carrito = HttpContext.Session.GetString("Carrito");
            var listaCarrito = string.IsNullOrEmpty(carrito)
                ? new List<dynamic>()
                : JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            listaCarrito = listaCarrito.Where(item =>
                !(item.EspacioId == espacioId &&
                  (DateTime)item.FechaInicio == fechaInicio &&
                  (DateTime)item.FechaFin == fechaFin)).ToList();

            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(listaCarrito));
            return RedirectToAction("Index");
        }

        public IActionResult Resumen()
        {
            var carrito = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carrito))
            {
                return View(new List<dynamic>());
            }

            var listaCarrito = JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            var descuentoStr = HttpContext.Session.GetString("Descuento");
            double descuento = string.IsNullOrEmpty(descuentoStr) ? 0 : double.Parse(descuentoStr);

            ViewData["DescuentoAplicado"] = descuento;
            ViewData["Total"] = listaCarrito.Sum(item => (double)item.Precio) - descuento;

            return View(listaCarrito);
        }


        // Aumentar una hora al espacio en el carrito
        [HttpPost]
        public IActionResult Aumentar(int espacioId, DateTime fechaInicio, DateTime fechaFin)
        {
            var carrito = HttpContext.Session.GetString("Carrito");
            var listaCarrito = string.IsNullOrEmpty(carrito)
                ? new List<dynamic>()
                : JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            foreach (var item in listaCarrito)
            {
                if ((int)item.EspacioId == espacioId &&
                    (DateTime)item.FechaInicio == fechaInicio &&
                    (DateTime)item.FechaFin == fechaFin)
                {
                    // Aumentar una hora al tiempo de reserva
                    item.FechaFin = ((DateTime)item.FechaFin).AddHours(1);
                    item.Precio += _context.Espacios.First(e => e.EspacioId == espacioId).Precio; // Actualizar el precio
                    break;
                }
            }

            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(listaCarrito));
            return RedirectToAction("Index");
        }

        // Disminuir una hora al espacio en el carrito
        [HttpPost]
        public IActionResult Disminuir(int espacioId, DateTime fechaInicio, DateTime fechaFin)
        {
            var carrito = HttpContext.Session.GetString("Carrito");
            var listaCarrito = string.IsNullOrEmpty(carrito)
                ? new List<dynamic>()
                : JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            foreach (var item in listaCarrito)
            {
                if ((int)item.EspacioId == espacioId &&
                    (DateTime)item.FechaInicio == fechaInicio &&
                    (DateTime)item.FechaFin == fechaFin)
                {
                    // Evitar que el tiempo de reserva sea menor a una hora
                    if (((DateTime)item.FechaFin).Subtract(((DateTime)item.FechaInicio)).TotalHours > 1)
                    {
                        item.FechaFin = ((DateTime)item.FechaFin).AddHours(-1);
                        item.Precio -= _context.Espacios.First(e => e.EspacioId == espacioId).Precio; // Actualizar el precio
                    }
                    break;
                }
            }

            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(listaCarrito));
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult AplicarCupon(string cupon)
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson))
            {
                TempData["MensajeError"] = "El carrito está vacío.";
                return RedirectToAction("Resumen");
            }
            var carrito = JsonConvert.DeserializeObject<List<dynamic>>(carritoJson);
            double total = carrito.Sum(item => (double)item.Precio);
            double descuento = 0;
            if (cupon.ToUpper() == "MONTOYA")
            {
                descuento = total * 0.90; // Por ejemplo, un descuento fijo de 20
                HttpContext.Session.SetString("Descuento", descuento.ToString());
                TempData["Mensaje"] = $"Cupón aplicado. Descuento: S/ {descuento:F2}";
            }
            else
            {
                TempData["Mensaje"] = "Cupón no válido.";
            }

            return RedirectToAction("Resumen");
        }




    }
}
