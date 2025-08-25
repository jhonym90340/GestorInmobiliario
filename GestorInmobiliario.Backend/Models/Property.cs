using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes; // <-- ¡ESTA LÍNEA ES CRUCIAL Y DEBE ESTAR!


namespace GestorInmobiliario.Backend.Models
{
    // Representa el modelo de una Propiedad en la base de datos.
    [BsonIgnoreExtraElements] // Esta es la anotación que requiere el using y el paquete
    public class Property
    {
        // Identificador único de la propiedad en MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

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

        // Clave foránea que vincula esta propiedad a un propietario.
        // Debe coincidir con el Id del Owner.
        [BsonRepresentation(BsonType.ObjectId)] // Asegura que se almacene como ObjectId
        public string IdOwner { get; set; } = string.Empty;

              
        [BsonElement("PropertyImages")]
        public object? OldPropertyImages { get; set; }
    }
}