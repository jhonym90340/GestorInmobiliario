using NUnit.Framework;
using GestorInmobiliario.Backend.Models;
using System;

namespace GestorInmobiliario.Tests.Models
{
    [TestFixture]
    public class OwnerImageTests
    {
        [Test]
        public void OwnerImage_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var ownerImage = new OwnerImage();

            // Assert
            Assert.That(ownerImage.Enabled, Is.True);
            Assert.That(ownerImage.CreatedDate, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
        }

        [Test]
        public void OwnerImage_WithValidData_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var testDate = DateTime.UtcNow;
            var ownerImage = new OwnerImage
            {
                IdOwnerImage = "123",
                IdOwner = "owner456",
                File = "/images/test.jpg",
                Enabled = false,
                CreatedDate = testDate
            };

            // Assert
            Assert.That(ownerImage.IdOwnerImage, Is.EqualTo("123"));
            Assert.That(ownerImage.IdOwner, Is.EqualTo("owner456"));
            Assert.That(ownerImage.File, Is.EqualTo("/images/test.jpg"));
            Assert.That(ownerImage.Enabled, Is.False);
            Assert.That(ownerImage.CreatedDate, Is.EqualTo(testDate));
        }

        [Test]
        public void OwnerImage_EmptyStrings_ShouldBeAllowed()
        {
            // Arrange & Act
            var ownerImage = new OwnerImage
            {
                IdOwner = "",
                File = ""
            };

            // Assert - No debería lanzar excepciones
            Assert.That(ownerImage.IdOwner, Is.Empty);
            Assert.That(ownerImage.File, Is.Empty);
        }
    }
}