using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public string Detalles { get; set; }

        // Relación con el espacio
        public Espacio Espacio { get; set; }

        // Relación con el pago (opcional)
        public int? PagoId { get; set; } // Asegúrate de que sea nullable
        public Pago Pago { get; set; }
    }

}
