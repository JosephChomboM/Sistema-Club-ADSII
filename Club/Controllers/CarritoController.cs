using Club.Data;
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

        public IActionResult Resumen()
        {
            var carrito = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carrito))
            {
                return View(new List<dynamic>());
            }

            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                return RedirectToAction("Login", "Usuario");
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == int.Parse(usuarioId));
            if (usuario == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var listaCarrito = JsonConvert.DeserializeObject<List<dynamic>>(carrito);
            var resumenConClubes = new List<dynamic>();

            foreach (var item in listaCarrito)
            {
                int espacioId = (int)item.EspacioId;

                var espacio = _context.Espacios.Include(e => e.Lugar)
                                .FirstOrDefault(e => e.EspacioId == espacioId);

                if (espacio != null)
                {
                    resumenConClubes.Add(new
                    {
                        NombreEspacio = espacio.Nombre,
                        FechaInicio = item.FechaInicio,
                        FechaFin = item.FechaFin,
                        Precio = item.Precio,
                        NombreClub = espacio.Lugar.Nombre,
                        DireccionClub = espacio.Lugar.Direccion,
                        UsuarioEmail = usuario.Email,
                        UsuarioNombre = usuario.Nombre,
                        UsuarioApellido = usuario.Apellido,
                        UsuarioDNI = usuario.DNI,
                        UsuarioTelefono = usuario.Telefono,
                        UsuarioDireccion = usuario.Direccion
                    });
                }
            }

            ViewData["Total"] = resumenConClubes.Sum(r => (double)r.Precio);

            return View(resumenConClubes);
        }
    }
}
