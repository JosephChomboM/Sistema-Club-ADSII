// Models/Reservacion.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Club.Models
{
    public class Reservacion
    {
        [Key]
        public int ReservacionId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int EspacioId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; } // Fecha y hora de inicio

        [Required]
        public DateTime FechaFin { get; set; } // Fecha y hora de fin

        public string Detalles { get; set; }

        public Espacio Espacio { get; set; }
    }
}
