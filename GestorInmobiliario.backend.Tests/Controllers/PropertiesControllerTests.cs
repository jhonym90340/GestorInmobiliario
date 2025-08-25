using GestorInmobiliario.Backend.Controllers;
using GestorInmobiliario.Backend.DTOs;
using GestorInmobiliario.Backend.Interfaces;
using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Repositories;
using GestorInmobiliario.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorInmobiliario.Tests.Controllers
{
    [TestFixture]
    public class PropertiesControllerTests
    {
        private Mock<IPropertyRepository> _mockPropertyRepository;
        private Mock<IOwnerRepository> _mockOwnerRepository;
        private Mock<IPropertyImageRepository> _mockPropertyImageRepository;
        private Mock<IImageService> _mockImageService;
        private PropertiesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _mockPropertyImageRepository = new Mock<IPropertyImageRepository>();
            _mockImageService = new Mock<IImageService>();

            _controller = new PropertiesController(
                _mockPropertyRepository.Object,
                _mockOwnerRepository.Object,
                _mockPropertyImageRepository.Object,
                _mockImageService.Object
            );
        }

        [Test]
        public async Task GetProperties_WithFilters_ReturnsFilteredProperties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property { Id = "1", Name = "Property 1", Price = 100000 },
                new Property { Id = "2", Name = "Property 2", Price = 200000 }
            };

            _mockPropertyRepository.Setup(r => r.GetAllAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal?>(), It.IsAny<decimal?>()))
                .ReturnsAsync(properties);

            _mockPropertyImageRepository.Setup(r => r.GetByPropertyIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<PropertyImage>());

            // Act
            var result = await _controller.GetProperties("Property", "Address", 50000, 250000);

            // Assert - CORREGIDO
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var propertyDtos = okResult.Value as List<PropertyDto>;
            Assert.That(propertyDtos.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetProperty_ExistingId_ReturnsProperty()
        {
            // Arrange
            var propertyId = "123";
            var property = new Property { Id = propertyId, Name = "Test Property" };

            _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
                .ReturnsAsync(property);

            _mockPropertyImageRepository.Setup(r => r.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(new List<PropertyImage>());

            // Act
            var result = await _controller.GetProperty(propertyId);

            // Assert - CORREGIDO
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetProperty_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var propertyId = "999";
            _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
                .ReturnsAsync((Property)null);

            // Act
            var result = await _controller.GetProperty(propertyId);

            // Assert - CORREGIDO
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateProperty_ValidData_ReturnsCreatedResult()
        {
            // Arrange
            var propertyCreateDto = new PropertyCreateDto
            {
                IdOwner = "owner123",
                Name = "Test Property",
                Address = "Test Address",
                Price = 150000,
                CodeInternal = "TEST001",
                Year = 2023
            };

            var owner = new Owner { IdOwner = "owner123", Name = "Test Owner" };
            var property = new Property
            {
                Id = "prop123",
                IdOwner = propertyCreateDto.IdOwner,
                Name = propertyCreateDto.Name,
                Address = propertyCreateDto.Address,
                Price = propertyCreateDto.Price,
                CodeInternal = propertyCreateDto.CodeInternal,
                Year = propertyCreateDto.Year
            };

            _mockOwnerRepository.Setup(r => r.GetByIdAsync(propertyCreateDto.IdOwner))
                .ReturnsAsync(owner);

            _mockPropertyRepository.Setup(r => r.AddAsync(It.IsAny<Property>()))
                .Returns(Task.CompletedTask)
                .Callback<Property>(p => p.Id = "prop123");

            // Act
            var result = await _controller.CreateProperty(propertyCreateDto);

            // Assert - CORREGIDO
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task CreateProperty_InvalidOwner_ReturnsBadRequest()
        {
            // Arrange
            var propertyCreateDto = new PropertyCreateDto
            {
                IdOwner = "invalid-owner",
                Name = "Test Property"
            };

            _mockOwnerRepository.Setup(r => r.GetByIdAsync(propertyCreateDto.IdOwner))
                .ReturnsAsync((Owner)null);

            // Act
            var result = await _controller.CreateProperty(propertyCreateDto);

            // Assert - CORREGIDO
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        // Pruebas adicionales que podrías agregar
        [Test]
        public async Task UpdateProperty_ValidData_ReturnsNoContent()
        {
            // Arrange
            var propertyId = "123";
            var propertyUpdateDto = new PropertyUpdateDto
            {
                IdProperty = propertyId,
                IdOwner = "owner123",
                Name = "Updated Property",
                Address = "Updated Address",
                Price = 200000,
                CodeInternal = "UPD001",
                Year = 2024
            };

            var existingProperty = new Property { Id = propertyId, Name = "Original Property" };
            var owner = new Owner { IdOwner = "owner123", Name = "Test Owner" };

            _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
                .ReturnsAsync(existingProperty);

            _mockOwnerRepository.Setup(r => r.GetByIdAsync(propertyUpdateDto.IdOwner))
                .ReturnsAsync(owner);

            _mockPropertyRepository.Setup(r => r.UpdateAsync(propertyId, It.IsAny<Property>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProperty(propertyId, propertyUpdateDto);

            // Assert - CORREGIDO
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteProperty_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var propertyId = "123";
            var property = new Property { Id = propertyId, Name = "Test Property" };

            _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
                .ReturnsAsync(property);

            _mockPropertyImageRepository.Setup(r => r.DeleteByPropertyIdAsync(propertyId))
                .Returns(Task.CompletedTask);

            _mockPropertyRepository.Setup(r => r.DeleteAsync(propertyId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProperty(propertyId);

            // Assert - CORREGIDO
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}