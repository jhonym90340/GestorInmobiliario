using GestorInmobiliario.Backend.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Repositories
{
    public interface IPropertyTraceRepository
    {
        Task<IEnumerable<PropertyTrace>> GetAllAsync();
        Task<PropertyTrace?> GetByIdAsync(string id);
        Task AddAsync(PropertyTrace propertyTrace);
        Task UpdateAsync(string id, PropertyTrace propertyTrace);
        Task DeleteAsync(string id);
    }
}