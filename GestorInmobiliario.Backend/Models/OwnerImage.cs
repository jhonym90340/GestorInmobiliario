using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GestorInmobiliario.Backend.Models
{
    public class OwnerImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdOwnerImage { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("idOwner")]
        public string IdOwner { get; set; } = string.Empty;

        [BsonElement("file")]
        public string File { get; set; } = string.Empty;

        [BsonElement("enabled")]
        public bool Enabled { get; set; } = true;

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
