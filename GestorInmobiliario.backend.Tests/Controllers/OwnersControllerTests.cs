using GestorInmobiliario.Backend.Controllers;
using GestorInmobiliario.Backend.DTOs;
using GestorInmobiliario.Backend.Interfaces;
using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorInmobiliario.Tests.Controllers
{
    [TestFixture]
    public class OwnersControllerTests
    {
        private Mock<IMongoClient> _mockMongoClient;
        private Mock<IMongoDatabase> _mockDatabase;
        private Mock<IMongoCollection<Owner>> _mockOwnersCollection;
        private Mock<IOwnerImageRepository> _mockOwnerImageRepository;
        private Mock<IImageService> _mockImageService;
        private OwnersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockMongoClient = new Mock<IMongoClient>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockOwnersCollection = new Mock<IMongoCollection<Owner>>();
            _mockOwnerImageRepository = new Mock<IOwnerImageRepository>();
            _mockImageService = new Mock<IImageService>();

            _mockMongoClient.Setup(c => c.GetDatabase(It.IsAny<string>(), null))
                .Returns(_mockDatabase.Object);
            _mockDatabase.Setup(d => d.GetCollection<Owner>(It.IsAny<string>(), null))
                .Returns(_mockOwnersCollection.Object);

            _controller = new OwnersController(
                _mockMongoClient.Object,
                _mockOwnerImageRepository.Object,
                _mockImageService.Object
            );
        }

        [Test]
        public async Task CreateOwnerWithImage_ValidData_ReturnsCreatedResult()
        {
            // Arrange
            var ownerCreateDto = new OwnerCreateWithImageDto
            {
                Name = "Test Owner",
                Address = "Test Address",
                Birthday = DateTime.Now.AddYears(-30)
            };

            var owner = new Owner
            {
                IdOwner = "123",
                Name = ownerCreateDto.Name,
                Address = ownerCreateDto.Address,
                Birthday = ownerCreateDto.Birthday
            };

            _mockOwnersCollection.Setup(c => c.InsertOneAsync(owner, null, default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateOwnerWithImage(ownerCreateDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.That(createdAtResult.ActionName, Is.EqualTo(nameof(_controller.GetOwnerById)));
        }

        [Test]
        public async Task GetOwners_ReturnsOkResultWithOwners()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner { IdOwner = "1", Name = "Owner 1" },
                new Owner { IdOwner = "2", Name = "Owner 2" }
            };

            var asyncCursor = new Mock<IAsyncCursor<Owner>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(owners);

            _mockOwnersCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<FindOptions<Owner, Owner>>(), default))
                .ReturnsAsync(asyncCursor.Object);

            _mockOwnerImageRepository.Setup(r => r.GetByOwnerIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<OwnerImage>());

            // Act
            var result = await _controller.GetOwners();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<Owner>>());
        }

        [Test]
        public async Task GetOwnerById_ExistingId_ReturnsOwner()
        {
            // Arrange
            var ownerId = "123";
            var owner = new Owner { IdOwner = ownerId, Name = "Test Owner" };

            var asyncCursor = new Mock<IAsyncCursor<Owner>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(new List<Owner> { owner });

            _mockOwnersCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<FindOptions<Owner, Owner>>(), default))
                .ReturnsAsync(asyncCursor.Object);

            _mockOwnerImageRepository.Setup(r => r.GetByOwnerIdAsync(ownerId))
                .ReturnsAsync(new List<OwnerImage>());

            // Act
            var result = await _controller.GetOwnerById(ownerId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetOwnerById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var ownerId = "999";

            var asyncCursor = new Mock<IAsyncCursor<Owner>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(default))
                .ReturnsAsync(false);

            _mockOwnersCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<FindOptions<Owner, Owner>>(), default))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _controller.GetOwnerById(ownerId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task UploadOwnerImage_ValidFile_ReturnsOkResult()
        {
            // Arrange
            var ownerId = "123";
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.FileName).Returns("test.jpg");

            var owner = new Owner { IdOwner = ownerId, Name = "Test Owner" };

            var asyncCursor = new Mock<IAsyncCursor<Owner>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(new List<Owner> { owner });

            _mockOwnersCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<FindOptions<Owner, Owner>>(), default))
                .ReturnsAsync(asyncCursor.Object);

            _mockImageService.Setup(s => s.SaveImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("/images/test.jpg");

            _mockOwnerImageRepository.Setup(r => r.ImageExistsAsync(ownerId, It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockOwnerImageRepository.Setup(r => r.AddImageAsync(It.IsAny<OwnerImage>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UploadOwnerImage(ownerId, mockFile.Object);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}