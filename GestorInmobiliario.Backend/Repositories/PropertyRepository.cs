using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson; // Necesario para BsonRegularExpression
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Repositories
{
    // Implementación concreta de IPropertyRepository para interactuar con MongoDB.
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoCollection<Property> _propertiesCollection;

        public PropertyRepository(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _propertiesCollection = database.GetCollection<Property>(mongoDBSettings.Value.CollectionNameProperty);
        }

        // Constructor para testing 
        public PropertyRepository(IMongoCollection<Property> propertiesCollection)
        {
            _propertiesCollection = propertiesCollection;
        }

        public async Task<IEnumerable<Property>> GetAllAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice)
        {
            var filterBuilder = Builders<Property>.Filter;
            var filter = filterBuilder.Empty; // Filtro inicial vacío

            if (!string.IsNullOrWhiteSpace(name))
            {
                // Filtra por nombre usando una expresión regular para búsqueda insensible a mayúsculas/minúsculas.
                filter &= filterBuilder.Regex(p => p.Name, new BsonRegularExpression($".*{name}.*", "i"));
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                // Filtra por dirección usando una expresión regular para búsqueda insensible a mayúsculas/minúsculas.
                filter &= filterBuilder.Regex(p => p.Address, new BsonRegularExpression($".*{address}.*", "i"));
            }

            if (minPrice.HasValue)
            {
                // Filtra propiedades con precio mayor o igual al mínimo especificado.
                filter &= filterBuilder.Gte(p => p.Price, minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                // Filtra propiedades con precio menor o igual al máximo especificado.
                filter &= filterBuilder.Lte(p => p.Price, maxPrice.Value);
            }

            // Aplica el filtro construido y devuelve los resultados.
            return await _propertiesCollection.Find(filter).ToListAsync();
        }

        public async Task<Property?> GetByIdAsync(string id)
        {
            return await _propertiesCollection.Find(property => property.Id == id).FirstOrDefaultAsync();
        }
        public async Task AddAsync(Property property)
        {
            await _propertiesCollection.InsertOneAsync(property);
        }

        public async Task UpdateAsync(string id, Property property)
        {
            await _propertiesCollection.ReplaceOneAsync(p => p.Id == id, property);
        }

        public async Task DeleteAsync(string id)
        {
            await _propertiesCollection.DeleteOneAsync(property => property.Id == id);
        }


        // NUEVO MÉTODO: Buscar propiedad por código interno
        public async Task<Property?> GetByCodeInternalAsync(string codeInternal)
        {
            return await _propertiesCollection
                .Find(property => property.CodeInternal == codeInternal)
                .FirstOrDefaultAsync();
        }

    }
}
