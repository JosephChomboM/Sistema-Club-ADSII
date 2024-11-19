// Models/Lugar.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Club.Models
{
    public class Lugar
    {
        [Key]
        public int LugarId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(255)]
        public string Direccion { get; set; }

        [Required]
        public string Descripcion { get; set; } // Breve descripción del club

        [Required]
        public string LogoUrl { get; set; } // URL de la imagen del logo del club

        public ICollection<Espacio> Espacios { get; set; }
    }
}
