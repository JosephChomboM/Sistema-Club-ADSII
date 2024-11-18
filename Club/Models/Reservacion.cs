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
        public DateTime FechaReservacion { get; set; }

        public string Detalles { get; set; }
    }
}
