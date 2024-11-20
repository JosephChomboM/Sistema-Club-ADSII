using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;

namespace Club.Controllers
{
    public class PagoController : Controller
    {
        public IActionResult Checkout()
        {
            // Leer el carrito de la sesión
            var carrito = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carrito))
            {
                return RedirectToAction("Index", "Carrito"); // Redirige si el carrito está vacío
            }

            var listaCarrito = JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            if (listaCarrito == null || !listaCarrito.Any())
            {
                return RedirectToAction("Index", "Carrito"); // Redirige si no hay elementos
            }

            var lineItems = new List<SessionLineItemOptions>();

            // Crear elementos del carrito para Stripe
            foreach (var item in listaCarrito)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Precio * 100), // Stripe usa centavos
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.NombreEspacio
                        }
                    },
                    Quantity = 1
                });
            }

            // Crear sesión de Stripe
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Pago", null, Request.Scheme),
                CancelUrl = Url.Action("Cancel", "Pago", null, Request.Scheme),
            };

            var service = new SessionService();
            Session session = service.Create(options);

            // Redirigir a la página de Stripe
            return Redirect(session.Url);
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
