using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;

namespace GestorInmobiliario.Tests.Models
{
    [TestFixture]
    public class PropertyImageTests
    {
        [Test]
        public void PropertyImage_WithValidData_ShouldBeValid()
        {
            // Arrange
            var propertyImage = new PropertyImage
            {
                IdProperty = "prop123",
                File = "/images/property1.jpg"
            };

            var validationContext = new ValidationContext(propertyImage);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(propertyImage, validationContext, validationResults, true);

            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(validationResults, Is.Empty);
        }

        [Test]
        public void PropertyImage_WithoutIdProperty_ShouldBeInvalid()
        {
            // Arrange
            var propertyImage = new PropertyImage
            {
                IdProperty = "", // ID vacío
                File = "/images/property1.jpg"
            };

            var validationContext = new ValidationContext(propertyImage);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(propertyImage, validationContext, validationResults, true);

            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            Assert.That(validationResults[0].ErrorMessage, Does.Contain("El ID de la propiedad es obligatorio"));
        }

        [Test]
        public void PropertyImage_WithoutFile_ShouldBeInvalid()
        {
            // Arrange
            var propertyImage = new PropertyImage
            {
                IdProperty = "prop123",
                File = "" // File vacío
            };

            var validationContext = new ValidationContext(propertyImage);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(propertyImage, validationContext, validationResults, true);

            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            Assert.That(validationResults[0].ErrorMessage, Does.Contain("La ruta del archivo de imagen es obligatoria"));
        }

        [Test]
        public void PropertyImage_DefaultEnabled_ShouldBeTrue()
        {
            // Arrange & Act
            var propertyImage = new PropertyImage();

            // Assert
            Assert.That(propertyImage.Enabled, Is.True);
        }
    }
}