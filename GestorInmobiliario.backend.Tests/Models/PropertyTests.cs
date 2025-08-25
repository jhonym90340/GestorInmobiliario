using NUnit.Framework;
using GestorInmobiliario.Backend.Models;

namespace GestorInmobiliario.Tests.Models
{
    [TestFixture]
    public class PropertyTests
    {
        [Test]
        public void Property_DefaultValues_ShouldBeEmptyStrings()
        {
            // Arrange & Act
            var property = new Property();

            // Assert
            Assert.That(property.Name, Is.Empty);
            Assert.That(property.Address, Is.Empty);
            Assert.That(property.CodeInternal, Is.Empty);
            Assert.That(property.IdOwner, Is.Empty);
            Assert.That(property.Price, Is.EqualTo(0));
            Assert.That(property.Year, Is.EqualTo(0));
        }

        [Test]
        public void Property_WithValidData_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var property = new Property
            {
                Id = "prop123",
                Name = "Casa en la playa",
                Address = "Av. Beach 123",
                Price = 250000.50m,
                CodeInternal = "CASA-001",
                Year = 2020,
                IdOwner = "owner456"
            };

            // Assert
            Assert.That(property.Id, Is.EqualTo("prop123"));
            Assert.That(property.Name, Is.EqualTo("Casa en la playa"));
            Assert.That(property.Address, Is.EqualTo("Av. Beach 123"));
            Assert.That(property.Price, Is.EqualTo(250000.50m));
            Assert.That(property.CodeInternal, Is.EqualTo("CASA-001"));
            Assert.That(property.Year, Is.EqualTo(2020));
            Assert.That(property.IdOwner, Is.EqualTo("owner456"));
        }

        [Test]
        public void Property_OldPropertyImages_CanBeNull()
        {
            // Arrange & Act
            var property = new Property
            {
                OldPropertyImages = null
            };

            // Assert
            Assert.That(property.OldPropertyImages, Is.Null);
        }
    }
}
