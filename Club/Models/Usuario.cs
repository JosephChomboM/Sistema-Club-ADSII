// Models/Usuario.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Club.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Contrasena { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
