using Azure.Storage.Queues;
using AzurePlayground.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzurePlayground.DocumentProcessor.Worker
{
    public class DocumentQueueProcessor
    {
        private readonly QueueClient _queueClient;
        private readonly QueueClient _poisonQueueClient;
        private readonly ILogger<DocumentQueueProcessor> _logger;

        private const int MaxAttempts = 3;

        public DocumentQueueProcessor([FromKeyedServices("document-processing")] QueueClient queueClient, [FromKeyedServices("document-processing-poison")] QueueClient poisonQueueClient, ILogger<DocumentQueueProcessor> logger)
        {
            _queueClient = queueClient;
            _poisonQueueClient = poisonQueueClient;
            _logger = logger;
        }

        public async Task ProcessMessageAsync(Azure.Storage.Queues.Models.QueueMessage message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Message received. MessageId: {MessageId}, DequeueCount: {DequeueCount}", message.MessageId, message.DequeueCount);

                if (message.DequeueCount >= MaxAttempts)
                {
                    await MoveToPoisonQueueAsync(message, cancellationToken);
                    return;
                }

                var documentMessage = JsonSerializer.Deserialize<DocumentProcessingMessageDTO>(message.Body.ToString());

                if (documentMessage is null)
                {
                    throw new InvalidOperationException("Unable to deserialize message.");
                }

                await ProcessDocumentAsync(documentMessage, cancellationToken);
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
                _logger.LogInformation("Message processed and removed from Queue. MessageId: {MessageId}", message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message. MessageId: {MessageId}, DequeueCount: {DequeueCount}", message.MessageId, message.DequeueCount);
            }
        }

        protected virtual async Task ProcessDocumentAsync(DocumentProcessingMessageDTO document, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting document processing. DocumentId: {DocumentId}, FileName: {FileName}", document.DocumentId, document.FileName);

            // Simulação de processamento
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            _logger.LogInformation("Document processed successfully. DocumentId: {DocumentId}", document.DocumentId);
        }

        private async Task MoveToPoisonQueueAsync(Azure.Storage.Queues.Models.QueueMessage message, CancellationToken cancellationToken)
        {
            await _poisonQueueClient.SendMessageAsync(message.Body.ToString(), cancellationToken);
            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);

            _logger.LogWarning("Message moved to Poison Queue. MessageId: {MessageId}, Attempts: {Attempts}", message.MessageId, message.DequeueCount);
        }
    }
}
