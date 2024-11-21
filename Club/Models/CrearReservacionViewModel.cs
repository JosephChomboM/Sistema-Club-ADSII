// Models/CrearReservacionViewModel.cs
using System;

namespace Club.Models
{
    public class CrearReservacionViewModel
    {
        public int EspacioId { get; set; }
        public string EspacioNombre { get; set; } // Nombre del espacio que se mostrará
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
