using GestorInmobiliario.Backend.DTOs;
using GestorInmobiliario.Backend.Models; 
using GestorInmobiliario.Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Para DateTime

namespace GestorInmobiliario.Backend.Controllers
{
    [Route("api/[controller]")] // Esto se resolverá a /api/PropertyTraces
    [ApiController]
    public class PropertyTracesController : ControllerBase
    {
        private readonly IPropertyTraceRepository _propertyTraceRepository;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyTracesController(IPropertyTraceRepository propertyTraceRepository, IPropertyRepository propertyRepository)
        {
            _propertyTraceRepository = propertyTraceRepository;
            _propertyRepository = propertyRepository;
        }

        // GET: api/PropertyTraces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyTraceDto>>> GetPropertyTraces()
        {
            var propertyTraces = await _propertyTraceRepository.GetAllAsync();

            var propertyTraceDtos = propertyTraces.Select(t => new PropertyTraceDto
            {
                IdPropertyTrace = t.Id, 
                DateSale = t.DateSale,
                Name = t.Name,
                Value = t.Value,
                Tax = t.Tax,
                IdProperty = t.IdProperty 
            }).ToList();

            return Ok(propertyTraceDtos);
        }

        // GET: api/PropertyTraces/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyTraceDto>> GetPropertyTrace(string id)
        {
            var propertyTrace = await _propertyTraceRepository.GetByIdAsync(id);

            if (propertyTrace == null)
            {
                return NotFound();
            }

            var propertyTraceDto = new PropertyTraceDto
            {
                IdPropertyTrace = propertyTrace.Id, 
                DateSale = propertyTrace.DateSale,
                Name = propertyTrace.Name,
                Value = propertyTrace.Value,
                Tax = propertyTrace.Tax,
                IdProperty = propertyTrace.IdProperty 
            };

            return Ok(propertyTraceDto);
        }

        // POST: api/PropertyTraces
        [HttpPost]
        public async Task<ActionResult<PropertyTraceDto>> CreatePropertyTrace([FromBody] PropertyTraceCreateDto propertyTraceCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var property = await _propertyRepository.GetByIdAsync(propertyTraceCreateDto.IdProperty);
            if (property == null)
            {
                ModelState.AddModelError("IdProperty", "La propiedad especificada no existe.");
                return BadRequest(ModelState);
            }

            var propertyTrace = new PropertyTrace
            {
                DateSale = propertyTraceCreateDto.DateSale,
                Name = propertyTraceCreateDto.Name,
                Value = propertyTraceCreateDto.Value,
                Tax = propertyTraceCreateDto.Tax,
                IdProperty = propertyTraceCreateDto.IdProperty
            };

            await _propertyTraceRepository.AddAsync(propertyTrace);

            var createdPropertyTraceDto = new PropertyTraceDto
            {
                IdPropertyTrace = propertyTrace.Id, 
                DateSale = propertyTrace.DateSale,
                Name = propertyTrace.Name,
                Value = propertyTrace.Value,
                Tax = propertyTrace.Tax,
                IdProperty = propertyTrace.IdProperty
            };

            return CreatedAtAction(nameof(GetPropertyTrace), new { id = createdPropertyTraceDto.IdPropertyTrace }, createdPropertyTraceDto);
        }

        // PUT: api/PropertyTraces/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePropertyTrace(string id, [FromBody] PropertyTraceUpdateDto propertyTraceUpdateDto)
        {
            if (id != propertyTraceUpdateDto.IdPropertyTrace)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del registro de trazabilidad en el cuerpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPropertyTrace = await _propertyTraceRepository.GetByIdAsync(id);
            if (existingPropertyTrace == null)
            {
                return NotFound();
            }

            var property = await _propertyRepository.GetByIdAsync(propertyTraceUpdateDto.IdProperty);
            if (property == null)
            {
                ModelState.AddModelError("IdProperty", "La propiedad especificada no existe.");
                return BadRequest(ModelState);
            }

            existingPropertyTrace.DateSale = propertyTraceUpdateDto.DateSale;
            existingPropertyTrace.Name = propertyTraceUpdateDto.Name;
            existingPropertyTrace.Value = propertyTraceUpdateDto.Value;
            existingPropertyTrace.Tax = propertyTraceUpdateDto.Tax;
            existingPropertyTrace.IdProperty = propertyTraceUpdateDto.IdProperty;

            await _propertyTraceRepository.UpdateAsync(id, existingPropertyTrace);

            return NoContent();
        }

        // DELETE: api/PropertyTraces/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropertyTrace(string id)
        {
            var propertyTrace = await _propertyTraceRepository.GetByIdAsync(id);
            if (propertyTrace == null)
            {
                return NotFound();
            }

            await _propertyTraceRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
