using Azure.Storage.Queues;
using AzurePlayground.Application.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.DocumentProcessor.Worker.Tests.Unit;

internal class TestableDocumentQueueProcessor : DocumentQueueProcessor
{
    public TestableDocumentQueueProcessor(QueueClient queueClient, QueueClient poisonQueueClient, ILogger<DocumentQueueProcessor> logger)
        : base(queueClient, poisonQueueClient, logger)
    {
    }

    protected override Task ProcessDocumentAsync(DocumentProcessingMessageDTO document, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
