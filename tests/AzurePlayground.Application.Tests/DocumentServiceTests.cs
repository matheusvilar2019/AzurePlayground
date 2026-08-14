using AutoMapper;
using AzurePlayground.Application.DTOs;
using AzurePlayground.Application.Interfaces;
using AzurePlayground.Application.Services;
using AzurePlayground.Domain.Entities;
using AzurePlayground.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzurePlayground.Application.Tests
{
    public class DocumentServiceTests
    {
        private readonly Mock<IDocumentRepository> _documentRepositoryMock;
        private readonly Mock<IDocumentStorage> _documentStorageMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly DocumentService _documentService;

        public DocumentServiceTests()
        {
            _documentRepositoryMock = new Mock<IDocumentRepository>();
            _documentStorageMock = new Mock<IDocumentStorage>();
            _mapperMock = new Mock<IMapper>();

            _documentService = new DocumentService(
                _mapperMock.Object,
                _documentRepositoryMock.Object,
                _documentStorageMock.Object);
        }

        [Fact]
        public async Task Add_ShouldUploadFileAndPersistDocument()
        {
            // Arrange
            var uploadDto = new DocumentUploadDTO
            {
                OriginalFileName = "document.pdf",
                ContentType = "application/pdf",
                Content = new MemoryStream(new byte[] { 1, 2, 3 })
            };

            var document = new Document(
                uploadDto.OriginalFileName,
                "generated-blob-name.pdf",
                "documents",
                uploadDto.ContentType,
                (int)uploadDto.Content.Length,
                "Active");

            var documentDto = new DocumentDTO
            {
                OriginalFileName = document.OriginalFileName,
                BlobName = document.BlobName,
                Container = document.Container,
                ContentType = document.ContentType,
                Size = document.Size,
                Status = document.Status
            };

            _documentRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Document>()))
                .ReturnsAsync(document);

            _mapperMock
                .Setup(x => x.Map<DocumentDTO>(It.IsAny<Document>()))
                .Returns(documentDto);

            // Act
            var result = await _documentService.Add(uploadDto);

            // Assert
            _documentStorageMock.Verify(
                x => x.UploadAsync(
                    uploadDto.Content,
                    It.IsAny<string>(),
                    uploadDto.ContentType,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _documentRepositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<Document>()),
                Times.Once);

            Assert.Equal(documentDto.OriginalFileName, result.OriginalFileName);
        }

        [Fact]
        public async Task Add_ShouldDeleteBlob_WhenRepositoryFails()
        {
            // Arrange
            var uploadDto = new DocumentUploadDTO
            {
                OriginalFileName = "document.pdf",
                ContentType = "application/pdf",
                Content = new MemoryStream(new byte[] { 1, 2, 3 })
            };

            _documentRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Document>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _documentService.Add(uploadDto));

            _documentStorageMock.Verify(
                x => x.DeleteAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Add_ShouldNotPersistDocument_WhenUploadFails()
        {
            // Arrange
            var uploadDto = new DocumentUploadDTO
            {
                OriginalFileName = "document.pdf",
                ContentType = "application/pdf",
                Content = new MemoryStream(new byte[] { 1, 2, 3 })
            };

            _documentStorageMock
                .Setup(x => x.UploadAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Storage error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _documentService.Add(uploadDto));

            _documentRepositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<Document>()),
                Times.Never);

            _documentStorageMock.Verify(
                x => x.DeleteAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Add_ShouldRejectFile_WhenExtensionIsInvalid()
        {
            // Arrange
            var uploadDto = new DocumentUploadDTO
            {
                OriginalFileName = "document.exe",
                ContentType = "application/octet-stream",
                Content = new MemoryStream(new byte[] { 1, 2, 3 })
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _documentService.Add(uploadDto));

            _documentStorageMock.Verify(
                x => x.UploadAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Add_ShouldRejectFile_WhenFileExceeds10MB()
        {
            // Arrange
            var content = new MemoryStream(
                new byte[10 * 1024 * 1024 + 1]);

            var uploadDto = new DocumentUploadDTO
            {
                OriginalFileName = "document.pdf",
                ContentType = "application/pdf",
                Content = content
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _documentService.Add(uploadDto));

            _documentStorageMock.Verify(
                x => x.UploadAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Add_ShouldUploadFile_WhenExtensionAndSizeAreValid()
        {
            // Arrange
            var uploadDto = new DocumentUploadDTO
            {
                OriginalFileName = "document.pdf",
                ContentType = "application/pdf",
                Content = new MemoryStream(new byte[] { 1, 2, 3 })
            };

            var document = new Document(
                uploadDto.OriginalFileName,
                "generated-name.pdf",
                "documents",
                uploadDto.ContentType,
                (int)uploadDto.Content.Length,
                "Active");

            _documentRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Document>()))
                .ReturnsAsync(document);

            // Act
            await _documentService.Add(uploadDto);

            // Assert
            _documentStorageMock.Verify(
                x => x.UploadAsync(
                    uploadDto.Content,
                    It.IsAny<string>(),
                    uploadDto.ContentType,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Add_ShouldRejectFile_WhenContentTypeDoesNotMatchExtension()
        {
            // Arrange
            var uploadDto = new DocumentUploadDTO
            {
                OriginalFileName = "document.pdf",
                ContentType = "image/png",
                Content = new MemoryStream(new byte[] { 1, 2, 3 })
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _documentService.Add(uploadDto));

            _documentStorageMock.Verify(
                x => x.UploadAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
