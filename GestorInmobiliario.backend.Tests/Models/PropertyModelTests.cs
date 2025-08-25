using NUnit.Framework;
using GestorInmobiliario.Backend.Models;

namespace GestorInmobiliario.backend.Tests.Models
{
    [TestFixture]
    public class PropertyModelTests
    {
        [Test]
        public void PropertyModel_DeberiaCrearObjeto()
        {
            // Test del modelo Property
            var property = new Property
            {
                Id = "1",
                Name = "Test Property",
                Address = "Test Address",
                Price = 250000
            };

            Assert.That(property.Name, Is.EqualTo("Test Property"));
            Assert.That(property.Price, Is.EqualTo(250000));
            Assert.That(property.Address, Is.EqualTo("Test Address"));
        }

        [Test]
        public void PropertyModel_WithDefaultValues_ShouldWork()
        {
            var property = new Property();

            Assert.That(property.Id, Is.Null);
            Assert.That(property.Name, Is.EqualTo(string.Empty)); // Corregido: espera string.Empty en lugar de null
            Assert.That(property.Price, Is.EqualTo(0));
        }
    }
}