using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Repositories
{
    // Implementación concreta de IPropertyTraceRepository para interactuar con MongoDB.
    public class PropertyTraceRepository : IPropertyTraceRepository
    {
        private readonly IMongoCollection<PropertyTrace> _propertyTracesCollection;

        public PropertyTraceRepository(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _propertyTracesCollection = database.GetCollection<PropertyTrace>(mongoDBSettings.Value.CollectionNamePropertyTrace);
        }

        public async Task<IEnumerable<PropertyTrace>> GetAllAsync()
        {
            return await _propertyTracesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<PropertyTrace?> GetByIdAsync(string id)
        {
            return await _propertyTracesCollection.Find(trace => trace.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(PropertyTrace propertyTrace)
        {
            await _propertyTracesCollection.InsertOneAsync(propertyTrace);
        }

        public async Task UpdateAsync(string id, PropertyTrace propertyTrace)
        {
            await _propertyTracesCollection.ReplaceOneAsync(trace => trace.Id == id, propertyTrace);
        }

        public async Task DeleteAsync(string id)
        {
            await _propertyTracesCollection.DeleteOneAsync(trace => trace.Id == id);
        }
    }
}

