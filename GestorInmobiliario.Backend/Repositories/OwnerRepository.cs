using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Repositories
{
    // Implementación concreta de IOwnerRepository para interactuar con MongoDB.
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IMongoCollection<Owner> _ownersCollection;

        public OwnerRepository(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _ownersCollection = database.GetCollection<Owner>(mongoDBSettings.Value.CollectionNameOwner);
        }

        // Constructor adicional para testing - permite inyectar IMongoCollection
        public OwnerRepository(IMongoCollection<Owner> ownersCollection)
        {
            _ownersCollection = ownersCollection;
        }


        public async Task<IEnumerable<Owner>> GetAllAsync()
        {
            return await _ownersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Owner?> GetByIdAsync(string id)
        {
            // Busca por el campo IdOwner, que es el PK
            return await _ownersCollection.Find(owner => owner.IdOwner == id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(Owner owner)
        {
            await _ownersCollection.InsertOneAsync(owner);
        }

        // Método para actualizar un propietario existente.
        public async Task UpdateAsync(string id, Owner owner)
        {
            // Reemplaza el documento completo con el nuevo objeto 'owner'
            await _ownersCollection.ReplaceOneAsync(o => o.IdOwner == id, owner);
        }

        // Método para eliminar un propietario por su ID.
        public async Task DeleteAsync(string id)
        {
            // Elimina el documento cuyo IdOwner coincida con el ID proporcionado
            await _ownersCollection.DeleteOneAsync(o => o.IdOwner == id);
        }
    }
}
