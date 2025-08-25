using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestorInmobiliario.Backend.DTOs
{
    public class OwnerCreateWithImageDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string Address { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        public IFormFile? PhotoFile { get; set; }
    }
}
