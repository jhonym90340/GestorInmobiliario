using System.ComponentModel.DataAnnotations; // Necesario para [Required] y [StringLength]


namespace GestorInmobiliario.Backend.DTOs
{
    // DTO para crear una nueva propiedad (usado en peticiones POST).
    // Incluye validaciones básicas.
    public class PropertyCreateDto
    {
        [Required(ErrorMessage = "El ID del propietario es obligatorio.")]
        public string IdOwner { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de la propiedad es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección de la propiedad es obligatoria.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "La dirección debe tener entre 5 y 200 caracteres.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio de la propiedad es obligatorio.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El código interno es obligatorio.")]
        public string CodeInternal { get; set; } = string.Empty;

        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100.")]
        public int Year { get; set; }

        
    }
}