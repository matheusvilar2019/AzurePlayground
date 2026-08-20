using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzurePlayground.Application.Storage;

namespace AzurePlayground.Application.DTOs
{
    public class DocumentAdminDTO
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string BlobName { get; set; }
        public string Container { get; set; }
        public DateTime UploadedAt { get; set; }
        public DocumentStorageInfo Storage { get; set; }
    }
}
