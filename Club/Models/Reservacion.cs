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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime FechaInicio { get; set; } // Fecha y hora de inicio

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime FechaFin { get; set; } // Fecha y hora de fin

        public string Detalles { get; set; }

        public Espacio Espacio { get; set; }
    }
}
