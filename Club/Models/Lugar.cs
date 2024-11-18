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

        public ICollection<Espacio> Espacios { get; set; }
    }
}
