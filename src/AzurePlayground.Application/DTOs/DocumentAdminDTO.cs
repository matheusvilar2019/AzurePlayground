using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.DTOs
{
    public class DocumentAdminDTO
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string BlobName { get; set; }
        public string Container { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        public string ETag { get; set; }
        public string Url { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string BlobType { get; set; }
        public string AccessTier { get; set; }
    }
}
