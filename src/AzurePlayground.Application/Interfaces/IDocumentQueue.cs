using AzurePlayground.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Interfaces;

public interface IDocumentQueue
{
    Task EnsureQueueExistsAsync(CancellationToken cancellationToken = default);
    Task SendMessageAsync(DocumentProcessingMessageDTO message, CancellationToken cancellationToken = default);
}
