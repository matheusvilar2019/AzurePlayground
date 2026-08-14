using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.DTOs
{
    public class DocumentUploadDTO
    {
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
    }
}
