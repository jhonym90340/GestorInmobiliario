using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GestorInmobiliario.Backend.Repositories
{
    // Interfaz para el repositorio de Propiedades, definiendo las operaciones CRUD y de filtrado.
    public interface IPropertyRepository
    {
        // Método para obtener todas las propiedades, permitiendo filtrar por nombre, dirección y rango de precio.
        Task<IEnumerable<Property>> GetAllAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice);

        // Método para obtener una propiedad por su identificador único.
        Task<Property?> GetByIdAsync(string id);


        Task<Property?> GetByCodeInternalAsync(string codeInternal); // ← NUEVO MÉTODO

        // Método para agregar una nueva propiedad.
        Task AddAsync(Property property);

        // Método para actualizar una propiedad existente.
        Task UpdateAsync(string id, Property property);

        // Método para eliminar una propiedad por su identificador único.
        Task DeleteAsync(string id);
    }
}
