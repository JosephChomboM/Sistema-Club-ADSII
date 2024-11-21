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

        [StringLength(100)]
        public string Apellido { get; set; }

        [StringLength(8, MinimumLength = 8)]
        public string DNI { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Contrasena { get; set; }

        [StringLength(15)]
        [Phone]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
