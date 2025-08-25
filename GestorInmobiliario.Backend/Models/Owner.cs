using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations; // ¡Importante para las validaciones!

namespace GestorInmobiliario.Backend.Models
{
    public class Owner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Especifica que el _id es un ObjectId
        // Aunque la propiedad se llama IdOwner en C#, [BsonId] asegura que se mapee a _id en MongoDB.
        // MongoDB siempre usa _id como su campo de identificador primario interno.
        public string? IdOwner { get; set; } // Representará el IdOwner de la tabla

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("photo")]
        public string? Photo { get; set; } // Campo para la URL o ruta de la foto, puede ser nulo

        [BsonElement("birthday")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; } // Campo para la fecha de cumpleaños, puede ser nulo
    }
}
