using System;

namespace GestorInmobiliario.Backend.DTOs
{
    // DTO para representar la información de un registro de trazabilidad de propiedad
    // que se envía al cliente (frontend).
    public class PropertyTraceDto
    {
        public string? IdPropertyTrace { get; set; } // El ID del registro de trazabilidad
        public DateTime DateSale { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
        public string IdProperty { get; set; } = string.Empty; // ID de la propiedad asociada
    }
}

