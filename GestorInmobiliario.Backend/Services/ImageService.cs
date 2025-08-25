using GestorInmobiliario.Backend.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace GestorInmobiliario.Backend.Services
{
    public class ImageService : IImageService
    {
        private readonly ImageSettings _imageSettings;

        public ImageService(IOptions<ImageSettings> imageSettings)
        {
            _imageSettings = imageSettings.Value;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, string subFolder = "")
        {
            // Validar extensión del archivo
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!_imageSettings.AllowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"Extensión de archivo no permitida. Extensiones permitidas: {string.Join(", ", _imageSettings.AllowedExtensions)}");
            }

            // Validar tamaño del archivo
            if (imageFile.Length > _imageSettings.MaxFileSizeMB * 1024 * 1024)
            {
                throw new ArgumentException($"El tamaño del archivo excede el límite de {_imageSettings.MaxFileSizeMB}MB");
            }

            // Crear carpeta específica si se especifica
            var uploadPath = string.IsNullOrEmpty(subFolder)
                ? _imageSettings.UploadPath
                : Path.Combine(_imageSettings.UploadPath, subFolder);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generar nombre único para el archivo
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath, uniqueFileName);

            // Guardar el archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Retornar la URL relativa de la imagen
            var relativePath = string.IsNullOrEmpty(subFolder)
                ? uniqueFileName
                : $"{subFolder}/{uniqueFileName}";

            return $"/images/{relativePath}";
        }

        public void DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl)) return;

                // Extraer el nombre del archivo de la URL
                var fileName = Path.GetFileName(imageUrl);
                if (string.IsNullOrEmpty(fileName)) return;

                // Buscar el archivo en todas las subcarpetas posibles
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // Buscar recursivamente el archivo
                var files = Directory.GetFiles(imagesFolder, fileName, SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    System.IO.File.Delete(files[0]);
                    Console.WriteLine($"Archivo eliminado: {files[0]}");
                }
                else
                {
                    Console.WriteLine($"Archivo no encontrado: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar archivo de imagen: {ex.Message}");
            }
        }
    }
}