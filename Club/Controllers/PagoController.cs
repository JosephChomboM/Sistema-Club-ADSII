using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using Club.Data;

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
            // Recuperar el carrito de la sesión
            var carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson))
            {
                return BadRequest("El carrito está vacío.");
            }

            var carrito = JsonConvert.DeserializeObject<List<dynamic>>(carritoJson);

            // Calcular el total
            long totalCentavos = 0;
            var nombresProductos = new List<string>();

            foreach (var item in carrito)
            {
                totalCentavos += (long)(item.Precio * 100); // Convertir a centavos
                nombresProductos.Add((string)item.NombreEspacio); // Agregar el nombre del espacio
            }

            var nombreProducto = nombresProductos.Count > 1
                ? $"{nombresProductos[0]} + {nombresProductos.Count - 1} más"
                : nombresProductos[0];

            // Crear una sesión de Stripe
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

        [HttpGet]
        [Route("Pago/Return")]
        public IActionResult Return(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return View("Error"); // Vista de error personalizada
            }

            // Obtén la sesión de Stripe
            var service = new Stripe.Checkout.SessionService();
            var session = service.Get(session_id);

            // Verifica si el pago fue exitoso
            if (session.PaymentStatus == "paid")
            {
                // Limpia el carrito
                HttpContext.Session.Remove("Carrito");

                // Redirige automáticamente a la vista ConsultarReservas
                return RedirectToAction("ConsultarReservas", "Reservacion");
            }
            else
            {
                // Maneja el caso de error o cancelación del pago
                return RedirectToAction("Resumen", "Carrito"); // Redirige al carrito
            }
        }



    }
}
