using System;

namespace Club.Models
{
    public class CarritoItem
    {
        public int Id { get; set; } // Identificador del espacio
        public string NombreEspacio { get; set; } // Nombre del espacio reservado
        public DateTime FechaInicio { get; set; } // Fecha y hora de inicio de la reserva
        public DateTime FechaFin { get; set; } // Fecha y hora de fin de la reserva
        public double Precio { get; set; } // Precio de la reserva
    }
}
