using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using GestorInmobiliario.Backend.Models;
using System.Collections.Generic;
using System;

namespace GestorInmobiliario.Tests.Models
{
    [TestFixture]
    public class OwnerTests
    {
        [Test]
        public void Owner_WithValidData_ShouldBeValid()
        {
            // Arrange
            var owner = new Owner
            {
                Name = "Juan Pérez",
                Address = "Calle 123, Ciudad",
                Birthday = new DateTime(1980, 1, 1)
            };

            var validationContext = new ValidationContext(owner);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(owner, validationContext, validationResults, true);

            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(validationResults, Is.Empty);
        }

        [Test]
        public void Owner_WithoutName_ShouldBeInvalid()
        {
            // Arrange
            var owner = new Owner
            {
                Name = "", // Nombre vacío
                Address = "Calle 123, Ciudad"
            };

            var validationContext = new ValidationContext(owner);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(owner, validationContext, validationResults, true);

            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            Assert.That(validationResults[0].ErrorMessage, Does.Contain("El nombre es obligatorio"));
        }

        [Test]
        public void Owner_WithoutAddress_ShouldBeInvalid()
        {
            // Arrange
            var owner = new Owner
            {
                Name = "Juan Pérez",
                Address = "" // Dirección vacía
            };

            var validationContext = new ValidationContext(owner);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(owner, validationContext, validationResults, true);

            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            Assert.That(validationResults[0].ErrorMessage, Does.Contain("La dirección es obligatoria"));
        }

        [Test]
        public void Owner_WithNullProperties_ShouldHandleCorrectly()
        {
            // Arrange
            var owner = new Owner
            {
                IdOwner = null,
                Photo = null,
                Birthday = null
            };

            // Act & Assert - No debería lanzar excepciones
            Assert.That(() =>
            {
                // Solo acceder a las propiedades, no asignarlas a string
                var id = owner.IdOwner;
                var photo = owner.Photo;
                var birthday = owner.Birthday;

                // Verificar que son null
                Assert.That(id, Is.Null);
                Assert.That(photo, Is.Null);
                Assert.That(birthday, Is.Null);
            }, Throws.Nothing);
        }

        [Test]
        public void Owner_BirthdayNullable_ShouldAcceptNull()
        {
            // Arrange
            var owner = new Owner
            {
                Name = "Test Owner",
                Address = "Test Address",
                Birthday = null // Birthday explícitamente null
            };

            // Act & Assert
            Assert.That(owner.Birthday, Is.Null);
            Assert.That(() =>
            {
                var temp = owner.Birthday; // Solo lectura, no asignación
            }, Throws.Nothing);
        }

        [Test]
        public void Owner_BirthdayWithValue_ShouldStoreCorrectly()
        {
            // Arrange
            var testDate = new DateTime(1990, 5, 15);
            var owner = new Owner
            {
                Name = "Test Owner",
                Address = "Test Address",
                Birthday = testDate
            };

            // Assert
            Assert.That(owner.Birthday, Is.EqualTo(testDate));
            Assert.That(owner.Birthday?.Year, Is.EqualTo(1990));
        }

        [Test]
        public void Owner_PhotoNullable_ShouldAcceptNullOrString()
        {
            // Arrange & Act
            var owner1 = new Owner { Photo = null };
            var owner2 = new Owner { Photo = "/images/photo.jpg" };
            var owner3 = new Owner { Photo = string.Empty };

            // Assert
            Assert.That(owner1.Photo, Is.Null);
            Assert.That(owner2.Photo, Is.EqualTo("/images/photo.jpg"));
            Assert.That(owner3.Photo, Is.Empty);
        }
    }
}