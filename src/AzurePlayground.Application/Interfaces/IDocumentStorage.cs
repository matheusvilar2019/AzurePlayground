using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Interfaces
{
    public interface IDocumentStorage
    {
        Task UploadAsync(
            Stream content,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default);

        Task<Stream> DownloadAsync(
            string fileName,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string fileName,
            CancellationToken cancellationToken = default);
    }
}
