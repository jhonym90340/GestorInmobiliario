using GestorInmobiliario.Backend.DTOs;
using GestorInmobiliario.Backend.Interfaces; // Necesario para IPropertyImageRepository
using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO; // Necesario para Path y FileStream
using System; // Necesario para Guid
using GestorInmobiliario.Backend.Services;

namespace GestorInmobiliario.Backend.Controllers
{
    [Route("api/[controller]")] // Esto se resolverá a "api/Properties"
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPropertyImageRepository _propertyImageRepository; // ¡Inyectar el nuevo repositorio de imágenes!
        private readonly IImageService _imageService;
        public PropertiesController(IPropertyRepository propertyRepository, IOwnerRepository ownerRepository, IPropertyImageRepository propertyImageRepository, IImageService imageService)
        {
            _propertyRepository = propertyRepository;
            _ownerRepository = ownerRepository;
            _propertyImageRepository = propertyImageRepository; // Asignar el nuevo repositorio
            _imageService = imageService;
        }

        // GET: api/Properties (para listar todas las propiedades con filtros opcionales)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties(
            [FromQuery] string? name,
            [FromQuery] string? address,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var properties = await _propertyRepository.GetAllAsync(name, address, minPrice, maxPrice);

            var propertyDtos = new List<PropertyDto>();
            foreach (var property in properties)
            {
                // Obtener todas las imágenes asociadas a esta propiedad desde la colección PropertyImages
                var images = await _propertyImageRepository.GetByPropertyIdAsync(property.Id);
                // Mapear solo las URLs de los archivos
                var imageUrls = images.Select(img => img.File).ToList();

                propertyDtos.Add(new PropertyDto
                {
                    IdProperty = property.Id,
                    IdOwner = property.IdOwner,
                    Name = property.Name,
                    Address = property.Address,
                    Price = property.Price,
                    CodeInternal = property.CodeInternal,
                    Year = property.Year,
                    ImageUrls = imageUrls // Asignar la lista de URLs de imágenes
                });
            }

            return Ok(propertyDtos);
        }

        // GET: api/Properties/{id} (para obtener una propiedad específica por ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetProperty(string id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);

            if (property == null)
            {
                return NotFound();
            }

            // Obtener todas las imágenes asociadas a esta propiedad desde la colección PropertyImages
            var images = await _propertyImageRepository.GetByPropertyIdAsync(property.Id);
            var imageUrls = images.Select(img => img.File).ToList();

            var propertyDto = new PropertyDto
            {
                IdProperty = property.Id,
                IdOwner = property.IdOwner,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                ImageUrls = imageUrls
            };

            return Ok(propertyDto);
        }



        // POST: api/Properties
        [HttpPost]
        public async Task<ActionResult<PropertyDto>> CreateProperty([FromBody] PropertyCreateDto propertyCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // VALIDACIÓN DE CÓDIGO INTERNO DUPLICADO - NUEVA VALIDACIÓN
            var existingPropertyWithCode = await _propertyRepository.GetByCodeInternalAsync(propertyCreateDto.CodeInternal);
            if (existingPropertyWithCode != null)
            {
                ModelState.AddModelError("CodeInternal", "El código interno ya existe. Por favor, use un código único.");
                return BadRequest(ModelState);
            }


            var owner = await _ownerRepository.GetByIdAsync(propertyCreateDto.IdOwner);
            if (owner == null)
            {
                ModelState.AddModelError("IdOwner", "El propietario especificado no existe.");
                return BadRequest(ModelState);
            }

            var property = new Property
            {
                IdOwner = propertyCreateDto.IdOwner,
                Name = propertyCreateDto.Name,
                Address = propertyCreateDto.Address,
                Price = propertyCreateDto.Price,
                CodeInternal = propertyCreateDto.CodeInternal,
                Year = propertyCreateDto.Year,
            };

            await _propertyRepository.AddAsync(property);

            var createdPropertyDto = new PropertyDto
            {
                IdProperty = property.Id,
                IdOwner = property.IdOwner,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                ImageUrls = new List<string>()
            };

            return CreatedAtAction(nameof(GetProperty), new { id = createdPropertyDto.IdProperty }, createdPropertyDto);
        }



        // PUT: api/Properties/{id} (para actualizar una propiedad - SIN IMAGEN AQUÍ)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(string id, PropertyUpdateDto propertyUpdateDto) // ¡Ya no recibe IFormFile!
        {
            if (id != propertyUpdateDto.IdProperty)
            {
                return BadRequest("El ID de la ruta no coincide con el ID de la propiedad en el cuerpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // VALIDACIÓN DE CÓDIGO INTERNO DUPLICADO -NUEVA VALIDACIÓN
    var existingPropertyWithCode = await _propertyRepository.GetByCodeInternalAsync(propertyUpdateDto.CodeInternal);
            if (existingPropertyWithCode != null && existingPropertyWithCode.Id != id)
            {
                ModelState.AddModelError("CodeInternal", "El código interno ya existe en otra propiedad. Por favor, use un código único.");
                return BadRequest(ModelState);
            }


            var existingProperty = await _propertyRepository.GetByIdAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            var owner = await _ownerRepository.GetByIdAsync(propertyUpdateDto.IdOwner);
            if (owner == null)
            {
                ModelState.AddModelError("IdOwner", "El propietario especificado no existe.");
                return BadRequest(ModelState);
            }

            existingProperty.IdOwner = propertyUpdateDto.IdOwner;
            existingProperty.Name = propertyUpdateDto.Name;
            existingProperty.Address = propertyUpdateDto.Address;
            existingProperty.Price = propertyUpdateDto.Price;
            existingProperty.CodeInternal = propertyUpdateDto.CodeInternal;
            existingProperty.Year = propertyUpdateDto.Year;

            await _propertyRepository.UpdateAsync(id, existingProperty);

            return NoContent();
        }

        // DELETE: api/Properties/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(string id)
        {
            var propertyToDelete = await _propertyRepository.GetByIdAsync(id);
            if (propertyToDelete == null)
            {
                return NotFound();
            }

          
            await _propertyImageRepository.DeleteByPropertyIdAsync(id);

  
            await _propertyRepository.DeleteAsync(id);
            return NoContent();
        }
      

        [HttpPost("{propertyId}/upload-image")]
        public async Task<IActionResult> UploadImage(string propertyId, [FromForm] IFormFile imageFile)
        {
            try
            {
                Console.WriteLine($"Subiendo imagen para propiedad: {propertyId}");

                // Validar que se haya enviado un archivo
                if (imageFile == null || imageFile.Length == 0)
                {
                    Console.WriteLine("Error: No se envió archivo o está vacío");
                    return BadRequest("El archivo de imagen es requerido.");
                }

                Console.WriteLine($"Archivo recibido: {imageFile.FileName}, Tamaño: {imageFile.Length} bytes, Tipo: {imageFile.ContentType}");

                // Validar que la propiedad exista
                var property = await _propertyRepository.GetByIdAsync(propertyId);
                if (property == null)
                {
                    Console.WriteLine($"Error: Propiedad con ID {propertyId} no encontrada");
                    return NotFound($"Propiedad con ID {propertyId} no encontrada.");
                }

                // Validar tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    Console.WriteLine($"Error: Extensión no permitida: {fileExtension}");
                    return BadRequest($"Tipo de archivo no permitido. Extensiones permitidas: {string.Join(", ", allowedExtensions)}");
                }

                // Validar tamaño (máximo 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    Console.WriteLine($"Error: Archivo demasiado grande: {imageFile.Length} bytes");
                    return BadRequest("El archivo es demasiado grande. Tamaño máximo: 5MB");
                }

                // Configurar la carpeta de uploads
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "properties");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Console.WriteLine($"Creada carpeta: {uploadsFolder}");
                }

                // Generar nombre único
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Console.WriteLine($"Guardando imagen en: {filePath}");

                // Guardar el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Crear registro en la base de datos
                var newImage = new PropertyImage
                {
                    IdProperty = propertyId,
                    File = $"/images/properties/{uniqueFileName}", // Ruta relativa
                    Enabled = true
                };

                await _propertyImageRepository.AddImageAsync(newImage);

                Console.WriteLine("Imagen subida exitosamente");

                return Ok(new
                {
                    imageUrl = newImage.File,
                    imageId = newImage.IdPropertyImage,
                    message = "Imagen subida exitosamente"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir imagen: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }



        [HttpGet("{propertyId}/images")]
            public async Task<IActionResult> GetPropertyImages(string propertyId)
            {
                try
                {
                    var images = await _propertyImageRepository.GetByPropertyIdAsync(propertyId);
                    var imageUrls = images.Select(img => img.File).ToList();
                    return Ok(imageUrls);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al obtener imágenes: {ex.Message}");
                }
            }

            [HttpDelete("delete-image")]
            public async Task<IActionResult> DeleteImage([FromBody] DeleteImageDto deleteImageDto)
            {
                try
                {
                    // Eliminar el archivo físico
                    _imageService.DeleteImage(deleteImageDto.ImageUrl);

                    // Eliminar el registro de la base de datos
                    await _propertyImageRepository.DeleteImageByUrlAsync(deleteImageDto.PropertyId, deleteImageDto.ImageUrl);

                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al eliminar la imagen: {ex.Message}");
                }
            }






        }
}
