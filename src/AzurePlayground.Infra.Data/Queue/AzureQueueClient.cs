using Azure.Storage.Queues;
using AzurePlayground.Application.DTOs;
using AzurePlayground.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzurePlayground.Infra.Data.Queue;

public class AzureQueueClient : IDocumentQueue
{
    private readonly QueueClient _queueClient;

    public AzureQueueClient(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task EnsureQueueExistsAsync(CancellationToken cancellationToken = default)
    {
        await _queueClient.CreateIfNotExistsAsync(
            cancellationToken: cancellationToken);
    }

    public async Task SendMessageAsync(DocumentProcessingMessageDTO message, CancellationToken cancellationToken = default)
    {
        var messageJson = JsonSerializer.Serialize(message);

        await _queueClient.SendMessageAsync(
            messageJson,
            cancellationToken);
    }
}