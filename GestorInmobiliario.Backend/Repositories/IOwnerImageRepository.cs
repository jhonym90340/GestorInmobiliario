using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Interfaces
{
    public interface IOwnerImageRepository
    {
        Task<List<OwnerImage>> GetByOwnerIdAsync(string ownerId);
        Task<OwnerImage> GetByIdAsync(string imageId);
        Task AddImageAsync(OwnerImage image);
        Task UpdateAsync(string imageId, OwnerImage image);
        Task DeleteImageByUrlAsync(string ownerId, string imageUrl);
        Task DeleteByOwnerIdAsync(string ownerId);
        Task<bool> ImageExistsAsync(string ownerId, string imageUrl);
    }
}
