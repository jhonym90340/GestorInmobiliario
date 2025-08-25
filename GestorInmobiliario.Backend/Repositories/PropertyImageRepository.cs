using GestorInmobiliario.Backend.Interfaces;
using GestorInmobiliario.Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Repositories
{
    // Implementación concreta de IPropertyImageRepository para interactuar con MongoDB.
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly IMongoCollection<PropertyImage> _propertyImagesCollection;


      

        public PropertyImageRepository(IMongoDatabase database)
        {
            // El nombre de la colección debe coincidir con el de base de datos
          
            _propertyImagesCollection = database.GetCollection<PropertyImage>("PropertyImages");
        }

       

        public async Task AddImageAsync(PropertyImage propertyImage)
        {
            await _propertyImagesCollection.InsertOneAsync(propertyImage);
        }

        public async Task CreateAsync(PropertyImage propertyImage)
        {
            await _propertyImagesCollection.InsertOneAsync(propertyImage);
        }

        public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(string propertyId)
        {
            // Busca todas las imágenes que coinciden con el IdProperty dado.
            var filter = Builders<PropertyImage>.Filter.Eq(i => i.IdProperty, propertyId);
            return await _propertyImagesCollection.Find(filter).ToListAsync();
        }

        public async Task DeleteByPropertyIdAsync(string propertyId)
        {
            // Elimina todas las imágenes asociadas a una propiedad.
            var filter = Builders<PropertyImage>.Filter.Eq(i => i.IdProperty, propertyId);
            await _propertyImagesCollection.DeleteManyAsync(filter);
        }

        public async Task DeleteImageByIdAsync(string imageId)
        {
            // Elimina una imagen específica por su IdPropertyImage.
            var filter = Builders<PropertyImage>.Filter.Eq(i => i.IdPropertyImage, imageId);
            await _propertyImagesCollection.DeleteOneAsync(filter);
        }


        public interface IPropertyImageRepository
        {
            
            Task DeleteImageByUrlAsync(string propertyId, string imageUrl);
        }


        public async Task DeleteImageByUrlAsync(string propertyId, string imageUrl)
        {
            var filter = Builders<PropertyImage>.Filter.And(
                Builders<PropertyImage>.Filter.Eq(i => i.IdProperty, propertyId),
                Builders<PropertyImage>.Filter.Eq(i => i.File, imageUrl)
            );
            await _propertyImagesCollection.DeleteOneAsync(filter);
        }




    }
}