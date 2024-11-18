// Models/Espacio.cs
using System.ComponentModel.DataAnnotations;

namespace Club.Models
{
    public class Espacio
    {
        [Key]
        public int EspacioId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public bool Disponible { get; set; }

        [Required]
        public int LugarId { get; set; }

        public Lugar Lugar { get; set; }
    }
}
