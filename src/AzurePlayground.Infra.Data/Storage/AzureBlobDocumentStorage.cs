using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzurePlayground.Application.Interfaces;
using AzurePlayground.Application.Storage;

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

        public async Task<DocumentStorageInfo> GetInfoAsync(
            string container,
            string blobName)
        {
            var blobClient =
                _containerClient.GetBlobClient(blobName);

            var properties = await blobClient.GetPropertiesAsync();

            return new DocumentStorageInfo
            {
                ContentLength = properties.Value.ContentLength,
                ContentType = properties.Value.ContentType,
                Metadata = properties.Value.Metadata,
                ETag = properties.Value.ETag.ToString(),
                Url = blobClient.Uri.ToString(),
                LastModified = properties.Value.LastModified,
                BlobType = properties.Value.BlobType.ToString(),
                AccessTier = properties.Value.AccessTier,
            };
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

            var metadata = new Dictionary<string, string>
            {
                ["documentType"] = "contract",
                ["department"] = "legal"
            };

            await blobClient.UploadAsync(
                content,
                new BlobHttpHeaders
                {
                    ContentType = contentType
                },
                metadata,
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
