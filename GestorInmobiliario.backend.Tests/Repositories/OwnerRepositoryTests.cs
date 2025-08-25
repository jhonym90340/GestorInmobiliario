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
    public class OwnerRepositoryTests
    {
        private Mock<IMongoCollection<Owner>> _mockCollection;
        private OwnerRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockCollection = new Mock<IMongoCollection<Owner>>();

            // Usar el constructor de testing que acepta IMongoCollection
            _repository = new OwnerRepository(_mockCollection.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllOwners()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner { IdOwner = ObjectId.GenerateNewId().ToString(), Name = "Owner 1" },
                new Owner { IdOwner = ObjectId.GenerateNewId().ToString(), Name = "Owner 2" }
            };

            var asyncCursor = new Mock<IAsyncCursor<Owner>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)   // Primer batch
                      .ReturnsAsync(false); // Fin de datos
            asyncCursor.Setup(_ => _.Current).Returns(owners);

            // Configurar el mock para que devuelva exactamente 2 elementos
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<FindOptions<Owner, Owner>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            var resultList = result.ToList();
            Assert.That(resultList, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsOwner()
        {
            // Arrange
            var ownerId = ObjectId.GenerateNewId().ToString();
            var owner = new Owner { IdOwner = ownerId, Name = "Test Owner" };

            var asyncCursor = new Mock<IAsyncCursor<Owner>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            asyncCursor.Setup(_ => _.Current).Returns(new List<Owner> { owner });

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<FindOptions<Owner, Owner>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync(ownerId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IdOwner, Is.EqualTo(ownerId));
        }

        [Test]
        public async Task AddAsync_ValidOwner_InsertsDocument()
        {
            // Arrange
            var owner = new Owner
            {
                IdOwner = ObjectId.GenerateNewId().ToString(),
                Name = "New Owner",
                Address = "Test Address"
            };

            // Configurar el mock para InsertOneAsync
            _mockCollection.Setup(x => x.InsertOneAsync(
                owner,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _repository.AddAsync(owner);

            // Assert - Verificar que se llamó al método
            _mockCollection.Verify(x => x.InsertOneAsync(
                owner,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ValidId_DeletesDocument()
        {
            // Arrange
            var ownerId = ObjectId.GenerateNewId().ToString();

            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockDeleteResult.Setup(r => r.DeletedCount).Returns(1);

            _mockCollection.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockDeleteResult.Object)
                .Verifiable();

            // Act
            await _repository.DeleteAsync(ownerId);

            // Assert
            _mockCollection.Verify(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ValidOwner_UpdatesDocument()
        {
            // Arrange
            var ownerId = ObjectId.GenerateNewId().ToString();
            var owner = new Owner
            {
                IdOwner = ownerId,
                Name = "Updated Owner"
            };

            var mockReplaceResult = new Mock<ReplaceOneResult>();
            mockReplaceResult.Setup(r => r.IsAcknowledged).Returns(true);
            mockReplaceResult.Setup(r => r.ModifiedCount).Returns(1);

            _mockCollection.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Owner>>(),
                owner,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReplaceResult.Object)
                .Verifiable();

            // Act
            await _repository.UpdateAsync(ownerId, owner);

            // Assert
            _mockCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Owner>>(),
                owner,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var asyncCursor = new Mock<IAsyncCursor<Owner>>();
            asyncCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false); // No hay datos
            asyncCursor.Setup(_ => _.Current).Returns(new List<Owner>());

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Owner>>(),
                It.IsAny<FindOptions<Owner, Owner>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync(ObjectId.GenerateNewId().ToString());

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}