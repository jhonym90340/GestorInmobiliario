using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System; // Para DateTime

namespace GestorInmobiliario.Backend.Models 
{
    public class PropertyTrace
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("dateSale")]
        public DateTime DateSale { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("value")]
        public decimal Value { get; set; }

        [BsonElement("tax")]
        public decimal Tax { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("idProperty")]
        public string IdProperty { get; set; } = string.Empty;
    }
}
