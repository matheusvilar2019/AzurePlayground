using Azure.Storage.Queues;
using AzurePlayground.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Infra.Data.Queue;

public class AzureQueueDocumentService : IDocumentQueueService
{
    private readonly QueueClient _queueClient;

    public AzureQueueDocumentService(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task EnsureQueueExistsAsync(
        CancellationToken cancellationToken = default)
    {
        await _queueClient.CreateIfNotExistsAsync(
            cancellationToken: cancellationToken);
    }
}