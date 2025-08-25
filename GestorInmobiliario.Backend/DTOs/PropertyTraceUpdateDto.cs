using System;
using System.ComponentModel.DataAnnotations;

namespace GestorInmobiliario.Backend.DTOs
{
    // DTO para actualizar un registro de trazabilidad existente.
    public class PropertyTraceUpdateDto
    {
        [Required(ErrorMessage = "El ID del registro de trazabilidad es obligatorio para la actualización.")]
        [StringLength(24, MinimumLength = 24, ErrorMessage = "El IdPropertyTrace debe ser un ObjectId de 24 caracteres.")]
        public string IdPropertyTrace { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha del evento es obligatoria.")]
        public DateTime DateSale { get; set; }

        [Required(ErrorMessage = "La descripción del evento es obligatoria.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 200 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El valor del evento es obligatorio.")]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "El valor debe ser un número positivo o cero.")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "El impuesto es obligatorio.")]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "El impuesto debe ser un número positivo o cero.")]
        public decimal Tax { get; set; }

        [Required(ErrorMessage = "El ID de la propiedad es obligatorio.")]
        [StringLength(24, MinimumLength = 24, ErrorMessage = "El IdProperty debe ser un ObjectId de 24 caracteres.")]
        public string IdProperty { get; set; } = string.Empty;
    }
}

