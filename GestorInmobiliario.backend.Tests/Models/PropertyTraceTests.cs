using NUnit.Framework;
using GestorInmobiliario.Backend.Models;
using System;

namespace GestorInmobiliario.Tests.Models
{
    [TestFixture]
    public class PropertyTraceTests
    {
        [Test]
        public void PropertyTrace_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var propertyTrace = new PropertyTrace();

            // Assert
            Assert.That(propertyTrace.Name, Is.Empty);
            Assert.That(propertyTrace.IdProperty, Is.Empty);
            Assert.That(propertyTrace.Value, Is.EqualTo(0));
            Assert.That(propertyTrace.Tax, Is.EqualTo(0));
            Assert.That(propertyTrace.DateSale, Is.EqualTo(DateTime.MinValue));
        }

        [Test]
        public void PropertyTrace_WithValidData_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var testDate = new DateTime(2023, 12, 15);
            var propertyTrace = new PropertyTrace
            {
                Id = "trace123",
                Name = "Venta inicial",
                Value = 200000.75m,
                Tax = 2000.25m,
                DateSale = testDate,
                IdProperty = "prop456"
            };

            // Assert
            Assert.That(propertyTrace.Id, Is.EqualTo("trace123"));
            Assert.That(propertyTrace.Name, Is.EqualTo("Venta inicial"));
            Assert.That(propertyTrace.Value, Is.EqualTo(200000.75m));
            Assert.That(propertyTrace.Tax, Is.EqualTo(2000.25m));
            Assert.That(propertyTrace.DateSale, Is.EqualTo(testDate));
            Assert.That(propertyTrace.IdProperty, Is.EqualTo("prop456"));
        }

        [Test]
        public void PropertyTrace_DecimalPrecision_ShouldBeMaintained()
        {
            // Arrange
            var propertyTrace = new PropertyTrace
            {
                Value = 123456.789m,
                Tax = 123.456m
            };

            // Assert - Verifica que los decimales se mantengan
            Assert.That(propertyTrace.Value, Is.EqualTo(123456.789m));
            Assert.That(propertyTrace.Tax, Is.EqualTo(123.456m));
        }
    }
}