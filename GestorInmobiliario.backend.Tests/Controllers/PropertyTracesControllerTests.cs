using GestorInmobiliario.Backend.Controllers;
using GestorInmobiliario.Backend.DTOs;
using GestorInmobiliario.Backend.Interfaces;
using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Repositories;
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
    public class PropertyTracesControllerTests
    {
        private Mock<IPropertyTraceRepository> _mockPropertyTraceRepository;
        private Mock<IPropertyRepository> _mockPropertyRepository;
        private PropertyTracesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockPropertyTraceRepository = new Mock<IPropertyTraceRepository>();
            _mockPropertyRepository = new Mock<IPropertyRepository>();

            _controller = new PropertyTracesController(
                _mockPropertyTraceRepository.Object,
                _mockPropertyRepository.Object
            );
        }

        [Test]
        public async Task GetPropertyTraces_ReturnsAllTraces()
        {
            // Arrange
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace { Id = "1", Name = "Trace 1" },
                new PropertyTrace { Id = "2", Name = "Trace 2" }
            };

            _mockPropertyTraceRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(traces);

            // Act
            var result = await _controller.GetPropertyTraces();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var traceDtos = okResult.Value as List<PropertyTraceDto>;
            Assert.That(traceDtos.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetPropertyTrace_ExistingId_ReturnsTrace()
        {
            // Arrange
            var traceId = "123";
            var trace = new PropertyTrace { Id = traceId, Name = "Test Trace" };

            _mockPropertyTraceRepository.Setup(r => r.GetByIdAsync(traceId))
                .ReturnsAsync(trace);

            // Act
            var result = await _controller.GetPropertyTrace(traceId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task CreatePropertyTrace_ValidData_ReturnsCreatedResult()
        {
            // Arrange
            var traceCreateDto = new PropertyTraceCreateDto
            {
                IdProperty = "prop123",
                Name = "Test Trace",
                Value = 150000,
                Tax = 1500,
                DateSale = DateTime.Now
            };

            var property = new Property { Id = "prop123", Name = "Test Property" };
            var trace = new PropertyTrace
            {
                Id = "trace123",
                IdProperty = traceCreateDto.IdProperty,
                Name = traceCreateDto.Name,
                Value = traceCreateDto.Value,
                Tax = traceCreateDto.Tax,
                DateSale = traceCreateDto.DateSale
            };

            _mockPropertyRepository.Setup(r => r.GetByIdAsync(traceCreateDto.IdProperty))
                .ReturnsAsync(property);

            _mockPropertyTraceRepository.Setup(r => r.AddAsync(It.IsAny<PropertyTrace>()))
                .Returns(Task.CompletedTask)
                .Callback<PropertyTrace>(t => t.Id = "trace123");

            // Act
            var result = await _controller.CreatePropertyTrace(traceCreateDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        }
    }
}