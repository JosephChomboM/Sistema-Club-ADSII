using Club.Data;
using Club.Models;
using Microsoft.AspNetCore.Mvc;
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

            var itemCarrito = new
            {
                EspacioId = espacio.EspacioId,
                NombreEspacio = espacio.Nombre,
                Precio = espacio.Precio,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var carrito = HttpContext.Session.GetString("Carrito");
            var listaCarrito = string.IsNullOrEmpty(carrito)
                ? new List<object>()
                : JsonConvert.DeserializeObject<List<object>>(carrito);

            listaCarrito.Add(itemCarrito);
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
        // Mostrar el resumen antes de proceder al pago
        public IActionResult Resumen()
        {
            var carrito = HttpContext.Session.GetString("Carrito");
            var listaCarrito = string.IsNullOrEmpty(carrito)
                ? new List<dynamic>()
                : JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            if (!listaCarrito.Any())
            {
                return RedirectToAction("Index"); // Si el carrito está vacío, redirigir al índice
            }

            // Calcular el total
            ViewData["Total"] = listaCarrito.Sum(item => (double)item.Precio);

            return View(listaCarrito);
        }

    }
}
