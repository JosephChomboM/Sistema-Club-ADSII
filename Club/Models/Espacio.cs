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
        public int LugarId { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public double Precio { get; set; }
        public Lugar Lugar { get; set; }
    }
}
