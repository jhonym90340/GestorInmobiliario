using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Services
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string subFolder = "");
        void DeleteImage(string imageUrl);
    }
}
