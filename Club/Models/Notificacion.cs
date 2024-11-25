// Models/Notificacion.cs
using System;
using System.ComponentModel.DataAnnotations;

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
    }
}
