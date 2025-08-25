using System.ComponentModel.DataAnnotations;

namespace GestorInmobiliario.Backend.DTOs
{
    public class DeleteOwnerImageDto
    {
        [Required(ErrorMessage = "El ID del propietario es obligatorio.")]
        public string OwnerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
