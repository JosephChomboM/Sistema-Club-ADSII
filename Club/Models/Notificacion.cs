// Models/Notificacion.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Club.Models
{
    public class Notificacion
    {
        [Key]
        public int NotificacionId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public bool Leido { get; set; } = false; // Indica si se ha leído la notificación

        public DateTime Fecha { get; set; } = DateTime.Now;

        // Relación con el usuario
        public Usuario Usuario { get; set; }

        // Relación con el admin que creó la notificación
        public int? AdminId { get; set; } // Opcional
        [ForeignKey("AdminId")]
        public Admin Admin { get; set; }
    }
}
