using AzurePlayground.Infra.Data.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzurePlayground.Infra.Data.Tests
{
    public class AzureBlobDocumentStorageTests
    {
        private readonly AzureBlobDocumentStorage _storage;

        public AzureBlobDocumentStorageTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString =
                configuration["AzureStorage:ConnectionString"];

            var containerName =
                configuration["AzureStorage:ContainerName"];

            _storage = new AzureBlobDocumentStorage(
                connectionString!,
                containerName!);
        }

        [Fact]
        public async Task UploadAsync_ShouldUploadFileToAzurite()
        {
            // Arrange
            const string fileName = "test-document.txt";
            const string contentType = "text/plain";
            const string content = "Hello Azurite!";

            await using var stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(content));

            // Act
            await _storage.UploadAsync(
                stream,
                fileName,
                contentType);

            // Assert
            var downloadedStream = await _storage.DownloadAsync(fileName);

            using var reader = new StreamReader(downloadedStream);

            var downloadedContent = await reader.ReadToEndAsync();

            Assert.Equal(content, downloadedContent);

            // Cleanup
            await _storage.DeleteAsync(fileName);
        }

        [Fact]
        public async Task DownloadAsync_ShouldReturnUploadedFile()
        {
            // Arrange
            const string fileName = "download-test.txt";
            const string content = "Conteúdo para teste de download";

            await using var uploadStream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(content));

            await _storage.UploadAsync(
                uploadStream,
                fileName,
                "text/plain");

            // Act
            await using var downloadStream =
                await _storage.DownloadAsync(fileName);

            using var reader = new StreamReader(downloadStream);

            var result = await reader.ReadToEndAsync();

            // Assert
            Assert.Equal(content, result);

            // Cleanup
            await _storage.DeleteAsync(fileName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteFile()
        {
            // Arrange
            const string fileName = "delete-test.txt";
            const string content = "Arquivo para exclusão";

            await using var uploadStream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(content));

            await _storage.UploadAsync(
                uploadStream,
                fileName,
                "text/plain");

            // Act
            await _storage.DeleteAsync(fileName);

            // Assert
            await Assert.ThrowsAsync<Azure.RequestFailedException>(
                async () => await _storage.DownloadAsync(fileName));
        }
    }
}
