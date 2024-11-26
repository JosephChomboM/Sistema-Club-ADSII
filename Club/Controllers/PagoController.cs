using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using Club.Data;
using Club.Models;

namespace Club.Controllers
{
    public class PagoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagoController(ApplicationDbContext context)
        {
            _context = context;
            StripeConfiguration.ApiKey = "sk_test_51QN2BoHINAMn88z1800oap6k5NTF3y8zZ0ALZOcDt6c437dW8vxX2CGvz7X6DnR4mFBiDNszXCkIXoGCVjVKAfAQ00g0sq3v5W";
        }

        [HttpPost]
        [Route("create-checkout-session")]
        public IActionResult CreateCheckoutSession()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson))
            {
                return BadRequest("El carrito está vacío.");
            }

            var carrito = JsonConvert.DeserializeObject<List<dynamic>>(carritoJson);

            // Calcular el total
            double total = carrito.Sum(item => (double)item.Precio);

            // Aplicar descuento si existe
            var descuentoStr = HttpContext.Session.GetString("Descuento");
            if (!string.IsNullOrEmpty(descuentoStr))
            {
                double descuento = double.Parse(descuentoStr);
                total -= descuento;
            }

            long totalCentavos = (long)(total * 100); // Convertir a centavos

            var nombresProductos = carrito.Select(item => (string)item.NombreEspacio).ToList();
            var nombreProducto = nombresProductos.Count > 1
                ? $"{nombresProductos[0]} + {nombresProductos.Count - 1} más"
                : nombresProductos[0];

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = totalCentavos,
                    Currency = "pen",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = nombreProducto
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                UiMode = "embedded",
                ReturnUrl = $"{Request.Scheme}://{Request.Host}/Pago/Return?session_id={{CHECKOUT_SESSION_ID}}",
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Json(new { clientSecret = session.ClientSecret });
        }


        [HttpGet]
        [Route("session-status")]
        public IActionResult SessionStatus(string session_id)
        {
            var service = new SessionService();
            var session = service.Get(session_id);

            return Json(new
            {
                status = session.Status,
                payment_status = session.PaymentStatus,
                session_id = session.Id
            });
        }

        [Route("Pago/Return")]
        public IActionResult Return(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return RedirectToAction("Error", "Home");
            }

            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus == "paid")
            {
                // Recuperar el carrito
                var carritoJson = HttpContext.Session.GetString("Carrito");
                if (string.IsNullOrEmpty(carritoJson))
                {
                    return RedirectToAction("Error", "Home");
                }

                var listaCarrito = JsonConvert.DeserializeObject<List<dynamic>>(carritoJson);
                var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

                // Recuperar el descuento desde la sesión
                var descuentoStr = HttpContext.Session.GetString("Descuento");
                decimal descuento = string.IsNullOrEmpty(descuentoStr) ? 0 : decimal.Parse(descuentoStr);

                // Calcular el total con descuento
                decimal total = listaCarrito.Sum(item => (decimal)item.Precio) - descuento;

                // Crear el pago (sin incluir el descuento como campo separado)
                var nuevoPago = new Pago
                {
                    UsuarioId = usuarioId,
                    Total = total,
                    Estado = "Confirmado",
                    FechaPago = DateTime.Now
                };
                _context.Pagos.Add(nuevoPago);
                _context.SaveChanges();

                // Asociar las reservaciones al pago
                foreach (var item in listaCarrito)
                {
                    var reservacion = new Reservacion
                    {
                        UsuarioId = usuarioId,
                        EspacioId = (int)item.EspacioId,
                        FechaInicio = (DateTime)item.FechaInicio,
                        FechaFin = (DateTime)item.FechaFin,
                        Detalles = "Reserva confirmada",
                        PagoId = nuevoPago.Id // Asociar el pago
                    };

                    _context.Reservaciones.Add(reservacion);

                    // Crear notificación
                    var espacio = _context.Espacios.FirstOrDefault(e => e.EspacioId == reservacion.EspacioId);
                    var notificacion = new Notificacion
                    {
                        UsuarioId = usuarioId,
                        Mensaje = $"Tu reserva para el espacio '{espacio.Nombre}' desde {reservacion.FechaInicio:yyyy-MM-dd HH:mm} hasta {reservacion.FechaFin:yyyy-MM-dd HH:mm} ha sido confirmada.",
                        Fecha = DateTime.Now
                    };

                    _context.Notificaciones.Add(notificacion);
                }

                _context.SaveChanges();

                // Limpiar el carrito y el descuento de la sesión
                HttpContext.Session.Remove("Carrito");
                HttpContext.Session.Remove("Descuento");

                return RedirectToAction("ConsultarReservas", "Reservacion");
            }

            return RedirectToAction("Error", "Home");
        }






    }
}
