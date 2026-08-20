using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.DTOs
{
    public class DocumentListDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public int Size { get; set; }
        public string ContentType { get; set; }
        public string Status { get; set; }
    }
}
