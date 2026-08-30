using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Interfaces;

public interface IDocumentQueueService
{
    Task EnsureQueueExistsAsync(CancellationToken cancellationToken = default);
}
