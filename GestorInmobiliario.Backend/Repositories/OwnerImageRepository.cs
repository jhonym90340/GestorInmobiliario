using GestorInmobiliario.Backend.Interfaces;
using GestorInmobiliario.Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Repositories
{
    public class OwnerImageRepository : IOwnerImageRepository
    {
        private readonly IMongoCollection<OwnerImage> _ownerImagesCollection;

        public OwnerImageRepository(IMongoDatabase database)
        {
            _ownerImagesCollection = database.GetCollection<OwnerImage>("OwnerImages");
        }

        public async Task<List<OwnerImage>> GetByOwnerIdAsync(string ownerId)
        {
            return await _ownerImagesCollection
                .Find(img => img.IdOwner == ownerId && img.Enabled)
                .SortByDescending(img => img.CreatedDate)
                .ToListAsync();
        }

        public async Task<OwnerImage> GetByIdAsync(string imageId)
        {
            return await _ownerImagesCollection
                .Find(img => img.IdOwnerImage == imageId)
                .FirstOrDefaultAsync();
        }

        public async Task AddImageAsync(OwnerImage image)
        {
            await _ownerImagesCollection.InsertOneAsync(image);
        }

        public async Task UpdateAsync(string imageId, OwnerImage image)
        {
            await _ownerImagesCollection
                .ReplaceOneAsync(img => img.IdOwnerImage == imageId, image);
        }

        public async Task DeleteImageByUrlAsync(string ownerId, string imageUrl)
        {
            await _ownerImagesCollection
                .DeleteOneAsync(img => img.IdOwner == ownerId && img.File == imageUrl);
        }

        public async Task DeleteByOwnerIdAsync(string ownerId)
        {
            await _ownerImagesCollection
                .DeleteManyAsync(img => img.IdOwner == ownerId);
        }

        public async Task<bool> ImageExistsAsync(string ownerId, string imageUrl)
        {
            return await _ownerImagesCollection
                .Find(img => img.IdOwner == ownerId && img.File == imageUrl && img.Enabled)
                .AnyAsync();
        }
    }
}
