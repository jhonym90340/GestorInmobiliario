using NUnit.Framework;
using Moq;
using MongoDB.Driver;
using GestorInmobiliario.Backend.Repositories;
using GestorInmobiliario.Backend.Models;
using GestorInmobiliario.Backend.Settings;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Bson;
using Microsoft.Extensions.Options;

namespace GestorInmobiliario.Tests.Repositories
{
    [TestFixture]
    public class PropertyTraceRepositoryTests
    {
        private Mock<IMongoCollection<PropertyTrace>> _mockCollection;
        private Mock<IMongoDatabase> _mockDatabase;
        private Mock<IMongoClient> _mockClient;
        private PropertyTraceRepository _repository;

        [SetUp]
        public void Setup()
        {
            // 1. Mockear la colección
            _mockCollection = new Mock<IMongoCollection<PropertyTrace>>();

            // 2. Mockear la base de datos para que devuelva la colección
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockDatabase.Setup(db => db.GetCollection<PropertyTrace>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                         .Returns(_mockCollection.Object);

            // 3. Mockear el cliente para que devuelva la base de datos
            _mockClient = new Mock<IMongoClient>();
            _mockClient.Setup(c => c.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                       .Returns(_mockDatabase.Object);

            // 4. Mockear IOptions<MongoDBSettings>
            var mockOptions = new Mock<IOptions<MongoDBSettings>>();
            mockOptions.Setup(o => o.Value).Returns(new MongoDBSettings
            {
                ConnectionURI = "mongodb://localhost:27017",
                DatabaseName = "GestorInmobiliarioDB", // Asegúrate de que este nombre coincida con tu base de datos real si la estás usando
                CollectionNamePropertyTrace = "PropertyTraces"
            });

            // 5. Instanciar el repositorio con un constructor de prueba que use el mock de la base de datos
            // Ya que el constructor original crea el cliente y la base de datos internamente,
            // la mejor práctica es crear un constructor de prueba en el repositorio para inyectar solo la colección.
            // Si eso no es posible, puedes modificar el repositorio para inyectar el cliente o la base de datos.

            // Si no puedes modificar el repositorio, la prueba unitaria es compleja.
            // Para simplificar, asumiremos que se agregó un constructor para pruebas.

            // Si el constructor original es el único, es mejor probar el "comportamiento" y no el "estado".
            // Para fines de esta solución, vamos a inyectar directamente el mock de la colección, 
            // aunque el constructor de tu repositorio no lo permita, para demostrar los mocks
            // para los métodos.
            // La solución más limpia sería un constructor del repositorio que tome un IMongoDatabase.

            // ***Solución más limpia para el repositorio***
            // Agrega un constructor a PropertyTraceRepository que tome un IMongoDatabase.
            // public PropertyTraceRepository(IMongoDatabase database) { ... }
            // Y luego usa este constructor en tu prueba:
            // _repository = new PropertyTraceRepository(_mockDatabase.Object);

            // Si no puedes modificar el repositorio, aquí está la solución que usa la inyección de opciones.
            _repository = new PropertyTraceRepository(mockOptions.Object);
        }

        // Tus pruebas individuales deben seguir como estaban, ya que el problema era el Setup.
    }
}