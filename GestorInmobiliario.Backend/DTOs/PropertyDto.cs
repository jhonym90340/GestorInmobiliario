using System.Collections.Generic;

namespace GestorInmobiliario.Backend.DTOs
{
    // DTO para representar una propiedad cuando se envía al frontend (lectura).
    // Contiene los campos necesarios para mostrar una vista resumida de la propiedad.
    public class PropertyDto
    {
        // Identificador único de la propiedad.
        public string? IdProperty { get; set; }

        // Identificador del propietario al que pertenece la propiedad (clave foránea).
        public string IdOwner { get; set; } = string.Empty;

        // Nombre de la propiedad.
        public string Name { get; set; } = string.Empty;

        // Dirección de la propiedad.
        public string Address { get; set; } = string.Empty;

        // Precio de la propiedad.
        public decimal Price { get; set; }

        // Código interno de la propiedad.
        public string CodeInternal { get; set; } = string.Empty;

        // Año de construcción o registro de la propiedad.
        public int Year { get; set; }

        // ¡CAMBIO CLAVE AQUÍ! Ahora es una lista de URLs de imágenes.
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}