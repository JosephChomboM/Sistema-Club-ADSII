using System.Collections.Generic;

namespace Club.Models.ViewModels
{
    public class LugarEspacioViewModel
    {
        public Lugar Lugar { get; set; }
        public List<Espacio> Espacios { get; set; } = new List<Espacio>(); // Inicialización predeterminada
    }
}

