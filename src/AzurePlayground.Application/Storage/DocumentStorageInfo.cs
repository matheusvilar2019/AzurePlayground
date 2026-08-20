using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Storage
{
    public class DocumentStorageInfo
    {
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ETag { get; set; }
        public string Url { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string BlobType { get; set; }
        public string AccessTier { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}
