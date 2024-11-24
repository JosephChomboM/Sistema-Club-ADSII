// Models/Pago.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Club.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public string Estado { get; set; } // "Pendiente", "Confirmado", etc.

        [Required]
        public DateTime FechaPago { get; set; }

        public Usuario Usuario { get; set; }
        public ICollection<Reservacion> Reservaciones { get; set; }
    }
}
