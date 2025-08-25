using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations; // Necesario para validaciones

namespace GestorInmobiliario.Backend.Models
{
    // Modelo que representa una imagen asociada a una propiedad, ahora como una colección separada.
    public class PropertyImage
    {
        // Identificador único de MongoDB para la imagen (PK de esta colección).
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdPropertyImage { get; set; } // Puede ser nulo al crear, se llena por MongoDB

        // Identificador de la propiedad a la que pertenece esta imagen (FK a Property).
        //  ObjectId requerido porque es una FK a una colección separada.
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "El ID de la propiedad es obligatorio.")]
        public string IdProperty { get; set; } = string.Empty;

        // URL o path del archivo de la imagen.
        [Required(ErrorMessage = "La ruta del archivo de imagen es obligatoria.")]
        public string File { get; set; } = string.Empty;

        // Indica si la imagen está habilitada o activa.
        public bool Enabled { get; set; } = true; // Por defecto, una imagen nueva está habilitada
    }
}