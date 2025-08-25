using NUnit.Framework;
using Moq;
using MongoDB.Driver;
using GestorInmobiliario.Backend.Repositories;
using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Bson;

namespace GestorInmobiliario.Tests.Repositories
{
    [TestFixture]
    public class PropertyImageRepositoryTests
    {
        private Mock<IMongoCollection<PropertyImage>> _mockCollection;
        private PropertyImageRepository _repository;

        [SetUp]
        
        public void Setup()
        {
            // Mockear IMongoCollection
            _mockCollection = new Mock<IMongoCollection<PropertyImage>>();

            // Mockear IMongoDatabase para devolver el mock de la colección
            var mockDatabase = new Mock<IMongoDatabase>();
            mockDatabase.Setup(db => db.GetCollection<PropertyImage>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                        .Returns(_mockCollection.Object);

            // Instanciar el repositorio con el mock correcto
            _repository = new PropertyImageRepository(mockDatabase.Object);
        }

        [Test]
        public async Task GetByPropertyIdAsync_ReturnsPropertyImages()
        {
            // Arrange
            var images = new List<PropertyImage>
            {
                new PropertyImage { IdPropertyImage = ObjectId.GenerateNewId().ToString(), IdProperty = "123", File = "img1.jpg" },
                new PropertyImage { IdPropertyImage = ObjectId.GenerateNewId().ToString(), IdProperty = "123", File = "img2.jpg" }
            };

            var asyncCursor = new Mock<IAsyncCursor<PropertyImage>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(images);

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<PropertyImage>>(),
                It.IsAny<FindOptions<PropertyImage, PropertyImage>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetByPropertyIdAsync("123");

            // Assert - CORREGIDO: Convertir a lista primero
            Assert.That(result, Is.Not.Null);
            var resultList = result.ToList(); // ← Conversión a lista
            Assert.That(resultList, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task AddImageAsync_ValidImage_InsertsDocument()
        {
            // Arrange
            var image = new PropertyImage
            {
                IdPropertyImage = ObjectId.GenerateNewId().ToString(),
                IdProperty = ObjectId.GenerateNewId().ToString(),
                File = "test.jpg"
            };

            _mockCollection.Setup(x => x.InsertOneAsync(
                image,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _repository.AddImageAsync(image);

            // Assert
            _mockCollection.Verify(x => x.InsertOneAsync(
                image,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteByPropertyIdAsync_ValidPropertyId_DeletesDocuments()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();

            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockDeleteResult.Setup(r => r.DeletedCount).Returns(2);

            _mockCollection.Setup(x => x.DeleteManyAsync(
                It.IsAny<FilterDefinition<PropertyImage>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockDeleteResult.Object)
                .Verifiable();

            // Act
            await _repository.DeleteByPropertyIdAsync(propertyId);

            // Assert
            _mockCollection.Verify(x => x.DeleteManyAsync(
                It.IsAny<FilterDefinition<PropertyImage>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteImageByUrlAsync_ValidData_DeletesDocument()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();
            var imageUrl = "/images/test.jpg";

            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockDeleteResult.Setup(r => r.DeletedCount).Returns(1);

            _mockCollection.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<PropertyImage>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockDeleteResult.Object)
                .Verifiable();

            // Act
            await _repository.DeleteImageByUrlAsync(propertyId, imageUrl);

            // Assert
            _mockCollection.Verify(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<PropertyImage>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}