using NUnit.Framework;
using Moq;
using MongoDB.Driver;
using MongoDB.Bson;
using GestorInmobiliario.Backend.Repositories;
using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace GestorInmobiliario.Tests.Repositories
{
    [TestFixture]
    public class PropertyRepositoryTests
    {
        private Mock<IMongoCollection<Property>> _mockCollection;
        private PropertyRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockCollection = new Mock<IMongoCollection<Property>>();

            // Usar el constructor de testing
            _repository = new PropertyRepository(_mockCollection.Object);
        }

        [Test]
        public async Task GetAllAsync_WithFilters_ReturnsFilteredProperties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property { Id = ObjectId.GenerateNewId().ToString(), Name = "Beach House", Price = 200000 },
                new Property { Id = ObjectId.GenerateNewId().ToString(), Name = "Mountain Cabin", Price = 150000 }
            };

            var asyncCursor = new Mock<IAsyncCursor<Property>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(properties);

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property, Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetAllAsync("Beach", "Address", 50000, 250000);

            // Assert
            Assert.That(result, Is.Not.Null);
            var resultList = result.ToList();
            Assert.That(resultList, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsProperty()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString(); // ID válido para MongoDB
            var property = new Property { Id = propertyId, Name = "Test Property" };

            var asyncCursor = new Mock<IAsyncCursor<Property>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(new List<Property> { property });

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property, Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync(propertyId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(propertyId));
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var asyncCursor = new Mock<IAsyncCursor<Property>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(new List<Property>());

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property, Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync(ObjectId.GenerateNewId().ToString());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddAsync_ValidProperty_InsertsDocument()
        {
            // Arrange
            var property = new Property
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Test Property",
                Address = "Test Address",
                Price = 150000,
                CodeInternal = "TEST001",
                Year = 2023,
                IdOwner = ObjectId.GenerateNewId().ToString()
            };

            _mockCollection.Setup(x => x.InsertOneAsync(
                property,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _repository.AddAsync(property);

            // Assert
            _mockCollection.Verify(x => x.InsertOneAsync(
                property,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ValidProperty_UpdatesDocument()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();
            var property = new Property
            {
                Id = propertyId,
                Name = "Updated Property",
                Address = "Updated Address",
                Price = 200000,
                CodeInternal = "UPD001",
                Year = 2024,
                IdOwner = ObjectId.GenerateNewId().ToString()
            };

            var mockReplaceResult = new Mock<ReplaceOneResult>();
            mockReplaceResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockReplaceResult.Setup(r => r.ModifiedCount).Returns(1);

            _mockCollection.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Property>>(),
                property,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReplaceResult.Object)
                .Verifiable();

            // Act
            await _repository.UpdateAsync(propertyId, property);

            // Assert
            _mockCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Property>>(),
                property,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ValidId_DeletesDocument()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();

            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockDeleteResult.Setup(r => r.DeletedCount).Returns(1);

            _mockCollection.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockDeleteResult.Object)
                .Verifiable();

            // Act
            await _repository.DeleteAsync(propertyId);

            // Assert
            _mockCollection.Verify(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}