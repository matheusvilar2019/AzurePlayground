using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzurePlayground.Application.Interfaces;

namespace AzurePlayground.Infra.Data.Storage
{
    public class AzureBlobDocumentStorage : IDocumentStorage
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobDocumentStorage(string connectionString, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);

            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task UploadAsync(
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            await _containerClient.CreateIfNotExistsAsync(
                cancellationToken: cancellationToken);

            var blobClient = _containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(
                content,
                new BlobHttpHeaders
                {
                    ContentType = contentType
                },
                cancellationToken: cancellationToken);
        }

        public async Task<Stream> DownloadAsync(
            string fileName,
            CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            var response = await blobClient.DownloadStreamingAsync(
                cancellationToken: cancellationToken);

            return response.Value.Content;
        }

        public async Task DeleteAsync(
            string fileName,
            CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync(
                cancellationToken: cancellationToken);
        }
    }
}
