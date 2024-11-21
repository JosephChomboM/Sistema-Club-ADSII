using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Newtonsoft.Json;

namespace Club.Controllers
{
    public class PagoController : Controller
    {
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

            // Crear la sesión de Stripe
            var options = new SessionCreateOptions
            {
                UiMode = "embedded",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = totalCentavos, // Total dinámico
                            Currency = "pen",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = nombreProducto // Nombre dinámico
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                RedirectOnCompletion = "never"
            };

            var service = new SessionService();
            var session = service.Create(options);

            // Devolver el client_secret al cliente para usar con Stripe.js
            return Json(new { clientSecret = session.ClientSecret });
        }

        public IActionResult Success()
        {
            ViewBag.Message = "¡Pago realizado con éxito!";
            return View();
        }

        public IActionResult Cancel()
        {
            ViewBag.Message = "El pago fue cancelado.";
            return View();
        }
    }
}
