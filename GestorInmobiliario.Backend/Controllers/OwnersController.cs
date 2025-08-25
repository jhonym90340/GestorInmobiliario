using GestorInmobiliario.Backend.DTOs;
using GestorInmobiliario.Backend.Interfaces;
using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Repositories;
using GestorInmobiliario.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorInmobiliario.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly IMongoCollection<Owner> _ownersCollection;
        private readonly IOwnerImageRepository _ownerImageRepository;
        private readonly IImageService _imageService;

        public OwnersController(IMongoClient mongoClient,
                              IOwnerImageRepository ownerImageRepository,
                              IImageService imageService)
        {
            var database = mongoClient.GetDatabase("GestorInmobiliarioDB");
            _ownersCollection = database.GetCollection<Owner>("Owners");
            _ownerImageRepository = ownerImageRepository;
            _imageService = imageService;
        }


        // POST: api/Owners/with-image
        [HttpPost("with-image")]
        public async Task<ActionResult<Owner>> CreateOwnerWithImage([FromForm] OwnerCreateWithImageDto ownerCreateDto)
        {
            try
            {
                // Crear el propietario primero
                var owner = new Owner
                {
                    Name = ownerCreateDto.Name,
                    Address = ownerCreateDto.Address,
                    Birthday = ownerCreateDto.Birthday
                };

                await _ownersCollection.InsertOneAsync(owner);

                // Si se envió una imagen, procesarla
                if (ownerCreateDto.PhotoFile != null && ownerCreateDto.PhotoFile.Length > 0)
                {
                    var imageUrl = await _imageService.SaveImageAsync(ownerCreateDto.PhotoFile, "owners");

                    var newImage = new OwnerImage
                    {
                        IdOwner = owner.IdOwner,
                        File = imageUrl,
                        Enabled = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    await _ownerImageRepository.AddImageAsync(newImage);

                    // Actualizar el propietario con la foto
                    owner.Photo = imageUrl;
                    await _ownersCollection.ReplaceOneAsync(o => o.IdOwner == owner.IdOwner, owner);
                }

                return CreatedAtAction(nameof(GetOwnerById), new { id = owner.IdOwner }, owner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear propietario: {ex.Message}");
            }
        }




        // GET: api/Owners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Owner>>> GetOwners()
        {
            try
            {
                var owners = await _ownersCollection.Find(_ => true).ToListAsync();

                // Obtener la imagen principal para cada propietario
                foreach (var owner in owners)
                {
                    var images = await _ownerImageRepository.GetByOwnerIdAsync(owner.IdOwner);
                    owner.Photo = images.FirstOrDefault()?.File;
                }

                return Ok(owners);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Owners/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Owner>> GetOwnerById(string id)
        {
            try
            {
                var owner = await _ownersCollection.Find(o => o.IdOwner == id).FirstOrDefaultAsync();
                if (owner == null)
                {
                    return NotFound();
                }

                // Obtener todas las imágenes del propietario
                var images = await _ownerImageRepository.GetByOwnerIdAsync(id);
                owner.Photo = images.FirstOrDefault()?.File;

                return Ok(owner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Owners
        [HttpPost]
        public async Task<ActionResult<Owner>> CreateOwner([FromBody] Owner newOwner)
        {
            try
            {
                await _ownersCollection.InsertOneAsync(newOwner);
                return CreatedAtAction(nameof(GetOwnerById), new { id = newOwner.IdOwner }, newOwner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear propietario: {ex.Message}");
            }
        }

        // POST: api/Owners/{ownerId}/upload-image
        [HttpPost("{ownerId}/upload-image")]
        public async Task<IActionResult> UploadOwnerImage(string ownerId, [FromForm] IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("El archivo de imagen es requerido.");
                }

                // Verificar que el propietario existe
                var owner = await _ownersCollection.Find(o => o.IdOwner == ownerId).FirstOrDefaultAsync();
                if (owner == null)
                {
                    return NotFound($"Propietario con ID {ownerId} no encontrado.");
                }

                // Guardar la imagen físicamente
                var imageUrl = await _imageService.SaveImageAsync(imageFile, "owners");

                // Verificar si la imagen ya existe
                var imageExists = await _ownerImageRepository.ImageExistsAsync(ownerId, imageUrl);
                if (imageExists)
                {
                    return Conflict("Esta imagen ya existe para este propietario.");
                }

                // Crear registro en la base de datos
                var newImage = new OwnerImage
                {
                    IdOwner = ownerId,
                    File = imageUrl,
                    Enabled = true,
                    CreatedDate = DateTime.UtcNow
                };

                await _ownerImageRepository.AddImageAsync(newImage);

                return Ok(new
                {
                    imageUrl = newImage.File,
                    imageId = newImage.IdOwnerImage,
                    message = "Imagen subida exitosamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al subir imagen: {ex.Message}");
            }
        }

        // GET: api/Owners/{ownerId}/images
        [HttpGet("{ownerId}/images")]
        public async Task<IActionResult> GetOwnerImages(string ownerId)
        {
            try
            {
                // Verificar que el propietario existe
                var owner = await _ownersCollection.Find(o => o.IdOwner == ownerId).FirstOrDefaultAsync();
                if (owner == null)
                {
                    return NotFound($"Propietario con ID {ownerId} no encontrado.");
                }

                var images = await _ownerImageRepository.GetByOwnerIdAsync(ownerId);
                var imageUrls = images.Select(img => new {
                    id = img.IdOwnerImage,
                    url = img.File,
                    createdDate = img.CreatedDate,
                    enabled = img.Enabled
                }).ToList();

                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener imágenes: {ex.Message}");
            }
        }

        // DELETE: api/Owners/delete-image
        [HttpDelete("delete-image")]
        public async Task<IActionResult> DeleteOwnerImage([FromBody] DeleteOwnerImageDto deleteImageDto)
        {
            try
            {
                // Verificar que el propietario existe
                var owner = await _ownersCollection.Find(o => o.IdOwner == deleteImageDto.OwnerId).FirstOrDefaultAsync();
                if (owner == null)
                {
                    return NotFound($"Propietario con ID {deleteImageDto.OwnerId} no encontrado.");
                }

                // Eliminar el archivo físico
                _imageService.DeleteImage(deleteImageDto.ImageUrl);

                // Eliminar el registro de la base de datos
                await _ownerImageRepository.DeleteImageByUrlAsync(deleteImageDto.OwnerId, deleteImageDto.ImageUrl);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la imagen: {ex.Message}");
            }
        }

        // PUT: api/Owners/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateOwner(string id, [FromBody] Owner updatedOwner)
        {
            try
            {
                var existingOwner = await _ownersCollection.Find(o => o.IdOwner == id).FirstOrDefaultAsync();
                if (existingOwner == null)
                {
                    return NotFound($"Propietario con ID {id} no encontrado.");
                }

                updatedOwner.IdOwner = existingOwner.IdOwner;
                await _ownersCollection.ReplaceOneAsync(o => o.IdOwner == id, updatedOwner);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar propietario: {ex.Message}");
            }
        }

        // DELETE: api/Owners/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteOwner(string id)
        {
            try
            {
                var existingOwner = await _ownersCollection.Find(o => o.IdOwner == id).FirstOrDefaultAsync();
                if (existingOwner == null)
                {
                    return NotFound($"Propietario con ID {id} no encontrado.");
                }

                // Eliminar todas las imágenes del propietario
                await _ownerImageRepository.DeleteByOwnerIdAsync(id);

                // Eliminar el propietario
                await _ownersCollection.DeleteOneAsync(o => o.IdOwner == id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar propietario: {ex.Message}");
            }
        }
    }
}