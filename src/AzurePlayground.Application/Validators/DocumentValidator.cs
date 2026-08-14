using AzurePlayground.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Validators
{
    public static class DocumentValidator
    {
        private const long MaxFileSize = 10 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png"
        };

        private static readonly Dictionary<string, string> AllowedFileTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [".pdf"] = "application/pdf",
                [".jpg"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".png"] = "image/png"
            };

        public static void Validate(DocumentUploadDTO document)
        {
            if (document.Content is null)
                throw new ArgumentException("File content is required.");

            if (document.Content.Length > MaxFileSize)
                throw new ArgumentException(
                    "File size cannot exceed 10 MB.");

            var extension = Path.GetExtension(
                document.OriginalFileName);

            if (!AllowedFileTypes.TryGetValue(extension, out var expectedContentType))
            {
                throw new ArgumentException("File type is not supported.");
            }

            if (!string.Equals(
                    document.ContentType,
                    expectedContentType,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Content type does not match the file extension.");
            }
        }
    }
}
