using NUnit.Framework;
using Moq;
using MongoDB.Driver;
using GestorInmobiliario.Backend.Repositories;
using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace GestorInmobiliario.Tests.Repositories
{
    [TestFixture]
    public class OwnerImageRepositoryTests
    {
        private Mock<IMongoCollection<OwnerImage>> _mockCollection;
        private Mock<IMongoDatabase> _mockDatabase;
        private OwnerImageRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockCollection = new Mock<IMongoCollection<OwnerImage>>();
            _mockDatabase = new Mock<IMongoDatabase>();

            _mockDatabase.Setup(d => d.GetCollection<OwnerImage>("OwnerImages", null))
                       .Returns(_mockCollection.Object);

            _repository = new OwnerImageRepository(_mockDatabase.Object);
        }

        [Test]
        public async Task GetByOwnerIdAsync_ReturnsFilteredImages()
        {
            // Arrange
            var images = new List<OwnerImage>
            {
                new OwnerImage { IdOwner = "123", File = "image1.jpg" },
                new OwnerImage { IdOwner = "123", File = "image2.jpg" }
            };

            var asyncCursor = new Mock<IAsyncCursor<OwnerImage>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(images);

            _mockCollection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<OwnerImage>>(),
                It.IsAny<FindOptions<OwnerImage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetByOwnerIdAsync("123");

            // Assert
            Assert.That(result, Is.Not.Null);
            var resultList = result.ToList();
            Assert.That(resultList, Has.Count.EqualTo(2));
            Assert.That(resultList.All(img => img.IdOwner == "123"), Is.True);
        }

        [Test]
        public async Task AddImageAsync_ValidImage_InsertsDocument()
        {
            // Arrange
            var image = new OwnerImage { IdOwner = "123", File = "test.jpg" };

            _mockCollection.Setup(x => x.InsertOneAsync(image, null, default))
                          .Returns(Task.CompletedTask);

            // Act
            await _repository.AddImageAsync(image);

            // Assert
            _mockCollection.Verify(x => x.InsertOneAsync(image, null, default), Times.Once);
        }


        // Métodos auxiliares para crear cursos mockeados
        private IAsyncCursor<OwnerImage> MockCursorWithData()
        {
            var mockCursor = new Mock<IAsyncCursor<OwnerImage>>();
            var images = new List<OwnerImage> { new OwnerImage { IdOwner = "123", File = "test.jpg", Enabled = true } };

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true)
                     .ReturnsAsync(false);
            mockCursor.Setup(_ => _.Current).Returns(images);

            return mockCursor.Object;
        }

        private IAsyncCursor<OwnerImage> MockCursorWithoutData()
        {
            var mockCursor = new Mock<IAsyncCursor<OwnerImage>>();
            var emptyImages = new List<OwnerImage>();

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);
            mockCursor.Setup(_ => _.Current).Returns(emptyImages);

            return mockCursor.Object;
        }

        [Test]
        public async Task DeleteImageByUrlAsync_ValidData_DeletesDocument()
        {
            // Arrange - CORRECCIÓN: Usar DeleteResult mockeado
            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockDeleteResult.Setup(r => r.DeletedCount).Returns(1);

            _mockCollection.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<OwnerImage>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockDeleteResult.Object);

            // Act
            await _repository.DeleteImageByUrlAsync("123", "test.jpg");

            // Assert
            _mockCollection.Verify(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<OwnerImage>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteByOwnerIdAsync_ValidOwnerId_DeletesDocuments()
        {
            // Arrange - CORRECCIÓN: Usar DeleteResult mockeado
            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockDeleteResult.Setup(r => r.DeletedCount).Returns(3);

            _mockCollection.Setup(x => x.DeleteManyAsync(
                It.IsAny<FilterDefinition<OwnerImage>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockDeleteResult.Object);

            // Act
            await _repository.DeleteByOwnerIdAsync("123");

            // Assert
            _mockCollection.Verify(x => x.DeleteManyAsync(
                It.IsAny<FilterDefinition<OwnerImage>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ExistingImage_ReturnsImage()
        {
            // Arrange
            var image = new OwnerImage { IdOwnerImage = "img123", IdOwner = "123", File = "test.jpg" };

            var asyncCursor = new Mock<IAsyncCursor<OwnerImage>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(new List<OwnerImage> { image });

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<OwnerImage>>(),
                It.IsAny<FindOptions<OwnerImage>>(),
                It.IsAny<CancellationToken>())) // CORRECCIÓN: It.IsAny en lugar de It.Any
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync("img123");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IdOwnerImage, Is.EqualTo("img123"));
            Assert.That(result.File, Is.EqualTo("test.jpg"));
        }

        [Test]
        public async Task UpdateAsync_ValidImage_UpdatesDocument()
        {
            // Arrange
            var image = new OwnerImage { IdOwnerImage = "img123", IdOwner = "123", File = "updated.jpg" };

            // Crear un ReplaceOneResult mockeado correctamente
            var mockReplaceResult = new Mock<ReplaceOneResult>();
            mockReplaceResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockReplaceResult.Setup(r => r.ModifiedCount).Returns(1);

            _mockCollection.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<OwnerImage>>(),
                image,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReplaceResult.Object);

            // Act
            await _repository.UpdateAsync("img123", image);

            // Assert
            _mockCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<OwnerImage>>(),
                image,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_NoDocumentFound_CompletesWithoutError()
        {
            // Arrange
            var image = new OwnerImage { IdOwnerImage = "nonexistent", IdOwner = "123", File = "updated.jpg" };

            // Simular que no se encontró documento para actualizar
            var mockReplaceResult = new Mock<ReplaceOneResult>();
            mockReplaceResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockReplaceResult.Setup(r => r.ModifiedCount).Returns(0); // Ningún documento modificado

            _mockCollection.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<OwnerImage>>(),
                image,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReplaceResult.Object);

            // Act & Assert - No debería lanzar excepción
            Assert.That(async () => await _repository.UpdateAsync("nonexistent", image), Throws.Nothing);
        }
    }
}