using AzurePlayground.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AzurePlayground.Domain.Entities
{
    public sealed class Document
    {
        public int Id { get; private set; }
        public string OriginalFileName { get; private set; }
        public string BlobName { get; private set; }
        public string Container { get; private set; }
        public string ContentType { get; private set; }
        public long Size { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public string Status { get; private set; }

        public Document(string originalFileName, string blobName, string container, string contentType, long size, string status)
        {
            UploadedAt = DateTime.Now;
            ValidateDomain(originalFileName, blobName, container, contentType, size, status);
        }

        public Document(int id, string originalFileName, string blobName, string container, string contentType, long size, string status)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value");
            Id = id;
            UploadedAt = DateTime.Now;
            ValidateDomain(originalFileName, blobName, container, contentType, size, status);
        }

        public void Update(string originalFileName, string blobName, string container, string contentType, long size, string status)
        {
            ValidateDomain(originalFileName, blobName, container, contentType, size, status);
        }

        private void ValidateDomain(string originalFileName, string blobName, string container, string contentType, long size, string status)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(originalFileName),
                "Invalid original file name. Original file name is required");

            DomainExceptionValidation.When(originalFileName.Length < 3,
               "Invalid original file name, too short, minimum 3 characters");

            DomainExceptionValidation.When(string.IsNullOrEmpty(blobName),
                "Invalid blobName. BlobName is required");

            DomainExceptionValidation.When(blobName.Length < 3,
               "Invalid blobName, too short, minimum 3 characters");

            DomainExceptionValidation.When(string.IsNullOrEmpty(container),
                "Invalid container. Container is required");

            DomainExceptionValidation.When(container.Length < 3,
               "Invalid container, too short, minimum 3 characters");

            DomainExceptionValidation.When(string.IsNullOrEmpty(contentType),
                "Invalid contentType. ContentType is required");

            DomainExceptionValidation.When(contentType.Length < 3,
               "Invalid contentType, too short, minimum 3 characters");

            DomainExceptionValidation.When(size < 0, "Invalid size value");

            DomainExceptionValidation.When(string.IsNullOrEmpty(status),
                "Invalid status. Status is required");

            DomainExceptionValidation.When(status.Length < 3,
               "Invalid status, too short, minimum 3 characters");

            DomainExceptionValidation.When(status.Length > 10,
               "Invalid status, too long, maximum 10 characters");

            OriginalFileName = originalFileName;
            BlobName = blobName;
            Container = container;
            ContentType = contentType;
            Size = size;
            Status = status;
        }
    }
}
