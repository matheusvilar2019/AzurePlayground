using Azure.Storage.Queues;
using AzurePlayground.Application.DTOs;
using System.Text.Json;
using System.Threading;

namespace AzurePlayground.DocumentProcessor.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly QueueClient _queueClient;
    private readonly QueueClient _poisonQueueClient;
    private const int _maxAttempts = 3;

    public Worker(ILogger<Worker> logger, [FromKeyedServices("document-processing")] QueueClient queueClient, [FromKeyedServices("document-processing-poison")] QueueClient poisonQueueClient)
    {
        _logger = logger;
        _queueClient = queueClient;
        _poisonQueueClient = poisonQueueClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _queueClient.ReceiveMessageAsync(visibilityTimeout: TimeSpan.FromSeconds(10), cancellationToken: stoppingToken);

            if (response.Value is not null)
            {
                var message = response.Value;

                _logger.LogInformation("Message received. MessageId: {MessageId}, DequeueCount: {DequeueCount}", message.MessageId, message.DequeueCount);
                var documentMessage = JsonSerializer.Deserialize<DocumentProcessingMessageDTO>(message.Body.ToString());

                if (documentMessage is not null)
                {
                    try
                    {
                        if (message.DequeueCount >= _maxAttempts)
                        {
                            await _poisonQueueClient.SendMessageAsync(message.Body.ToString(), stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                            _logger.LogWarning("Message moved to Poison Queue. MessageId: {MessageId}, Attempts: {Attempts}", message.MessageId, message.DequeueCount);

                            continue;
                        }

                        await ProcessDocumentAsync(documentMessage, stoppingToken);

                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                        _logger.LogInformation("Message removed from Queue. MessageId: {MessageId}", message.MessageId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing document. DocumentId: {DocumentId}, MessageId: {MessageId}", documentMessage.DocumentId, message.MessageId);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task ProcessDocumentAsync(DocumentProcessingMessageDTO document, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting document processing. DocumentId: {DocumentId}, FileName: {FileName}", document.DocumentId, document.FileName);

        // Simulação de processamento
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        _logger.LogInformation("Document processed successfully. DocumentId: {DocumentId}", document.DocumentId);
    }
}