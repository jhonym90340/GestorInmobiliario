using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Repositories
{
    // Define la interfaz para las operaciones del repositorio de propietarios.
    // Esto asegura que cualquier clase que implemente esta interfaz
    //  métodos para la gestión de datos de propietarios.
    public interface IOwnerRepository
    {
        // Obtiene todos los propietarios.
        Task<IEnumerable<Owner>> GetAllAsync();
        // Obtiene un propietario por su ID.
        Task<Owner?> GetByIdAsync(string id);
        // Agrega un nuevo propietario.
        Task AddAsync(Owner owner);
        // Actualiza un propietario existente por su ID.
        Task UpdateAsync(string id, Owner owner);
        // Elimina un propietario por su ID.
        Task DeleteAsync(string id);
    }
}
