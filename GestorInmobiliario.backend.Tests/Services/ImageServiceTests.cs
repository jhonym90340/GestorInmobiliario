using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GestorInmobiliario.Backend.Services;
using GestorInmobiliario.Backend.Settings;
using System;

namespace GestorInmobiliario.Tests.Services
{
    [TestFixture]
    public class ImageServiceTests
    {
        private ImageService _imageService;
        private Mock<IOptions<ImageSettings>> _mockOptions;
        private ImageSettings _settings;
        private string _testImagesPath;

        [SetUp]
        public void Setup()
        {
            // La configuración del servicio espera una ruta relativa, por ejemplo "images".
            _settings = new ImageSettings
            {
                UploadPath = Path.Combine("wwwroot", "images"),
                AllowedExtensions = new[] { ".jpg", ".png" },
                MaxFileSizeMB = 5
            };

            _mockOptions = new Mock<IOptions<ImageSettings>>();
            _mockOptions.Setup(o => o.Value).Returns(_settings);

            _imageService = new ImageService(_mockOptions.Object);

            // Calcula la ruta completa para los archivos de prueba
            _testImagesPath = Path.Combine(Directory.GetCurrentDirectory(), _settings.UploadPath);

            // Crea un directorio de prueba si no existe
            if (Directory.Exists(_testImagesPath))
            {
                Directory.Delete(_testImagesPath, true);
            }
            Directory.CreateDirectory(_testImagesPath);
        }

        [TearDown]
        public void Teardown()
        {
            // Limpia el directorio de prueba después de cada test
            if (Directory.Exists(_testImagesPath))
            {
                Directory.Delete(_testImagesPath, true);
            }
        }

        [Test]
        public async Task SaveImageAsync_ValidFile_ReturnsRelativeUrl()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var content = "This is a test image.";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(stream.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Callback<Stream, CancellationToken>((s, c) => stream.CopyTo(s))
                    .Returns(Task.CompletedTask);

            // Act
            var resultUrl = await _imageService.SaveImageAsync(mockFile.Object);

            // Assert
            Assert.That(resultUrl, Does.StartWith("/images/"));
            Assert.That(resultUrl, Does.EndWith(".jpg"));

            var savedFilePath = Path.Combine(_testImagesPath, Path.GetFileName(resultUrl));
            Assert.That(File.Exists(savedFilePath), Is.True);
        }

        [Test]
        public void SaveImageAsync_UnsupportedFileExtension_ThrowsArgumentException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.gif"); // Unsupported extension
            mockFile.Setup(f => f.Length).Returns(100);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _imageService.SaveImageAsync(mockFile.Object));
        }

        [Test]
        public void SaveImageAsync_FileTooLarge_ThrowsArgumentException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.jpg");
            mockFile.Setup(f => f.Length).Returns(_settings.MaxFileSizeMB * 1024 * 1024 + 1);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _imageService.SaveImageAsync(mockFile.Object));
        }

        [Test]
        public async Task SaveImageAsync_WithSubFolder_CreatesSubFolderAndReturnsCorrectUrl()
        {
            // Arrange
            var subFolder = "properties";
            var mockFile = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("test data"));

            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(stream.Length);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Callback<Stream, CancellationToken>((s, c) => stream.CopyTo(s))
                    .Returns(Task.CompletedTask);

            // Act
            var resultUrl = await _imageService.SaveImageAsync(mockFile.Object, subFolder);

            // Assert
            Assert.That(resultUrl, Does.StartWith($"/images/{subFolder}/"));

            var subDirectoryPath = Path.Combine(_testImagesPath, subFolder);
            Assert.That(Directory.Exists(subDirectoryPath), Is.True);

            var savedFilePath = Path.Combine(subDirectoryPath, Path.GetFileName(resultUrl));
            Assert.That(File.Exists(savedFilePath), Is.True);
        }

        [Test]
        public void DeleteImage_ExistingFile_DeletesFile()
        {
            // Arrange
            var tempFileName = "temp_delete.jpg";
            var tempFilePath = Path.Combine(_testImagesPath, tempFileName);
            File.WriteAllText(tempFilePath, "temp content"); // Create a dummy file to delete
            var imageUrl = $"/images/{tempFileName}";

            // Act
            _imageService.DeleteImage(imageUrl);

            // Assert
            Assert.That(File.Exists(tempFilePath), Is.False);
        }

        [Test]
        public void DeleteImage_NonExistingFile_DoesNothingAndHandlesGracefully()
        {
            // Arrange
            var imageUrl = "/images/non_existent.jpg";

            // Act & Assert
            Assert.DoesNotThrow(() => _imageService.DeleteImage(imageUrl));
        }
    }
}