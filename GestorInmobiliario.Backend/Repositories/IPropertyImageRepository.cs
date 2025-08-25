using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Interfaces
{
    // Define la interfaz para las operaciones del repositorio de imágenes de propiedades.
    public interface IPropertyImageRepository
    {
        // Agrega una nueva imagen a la base de datos.
        Task AddImageAsync(PropertyImage propertyImage);

        // Obtiene todas las imágenes asociadas a una propiedad específica por su ID.
        Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(string propertyId);

        // Opcional: Podrías añadir métodos para eliminar o actualizar imágenes si fuera necesario.
        Task DeleteByPropertyIdAsync(string propertyId); // Elimina todas las imágenes de una propiedad
        Task DeleteImageByIdAsync(string imageId); // Elimina una imagen específica
        Task DeleteImageByUrlAsync(string propertyId, string imageUrl);
        Task CreateAsync(PropertyImage propertyImage);
    }
}