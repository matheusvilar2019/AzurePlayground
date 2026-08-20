using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.DTOs
{
    public class DocumentDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The OriginalFileName is Required")]
        [MinLength(3)]
        [MaxLength(100)]
        [DisplayName("OriginalFileName")]
        public string OriginalFileName { get; set; }

        [Required(ErrorMessage = "The BlobName is Required")]
        [MinLength(3)]
        [MaxLength(100)]
        [DisplayName("BlobName")]
        public string BlobName { get; set; }

        [Required(ErrorMessage = "The Container is Required")]
        [MinLength(3)]
        [MaxLength(100)]
        [DisplayName("Container")]
        public string Container { get; set; }

        [Required(ErrorMessage = "The ContentType is Required")]
        [MinLength(3)]
        [MaxLength(100)]
        [DisplayName("ContentType")]
        public string ContentType { get; set; }

        [Required(ErrorMessage = "The Stock is Required")]
        [Range(1, 9999)]
        [DisplayName("Size")]
        public long Size { get; set; }


        [Required(ErrorMessage = "The UploadedAt is Required")]
        public DateTime UploadedAt { get; set; }

        [Required(ErrorMessage = "The Status is Required")]
        [MinLength(3)]
        [MaxLength(10)]
        [DisplayName("Status")]
        public string Status { get; set; }
    }
}
