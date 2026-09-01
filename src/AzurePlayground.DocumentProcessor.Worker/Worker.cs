using Azure.Storage.Queues;
using AzurePlayground.Application.DTOs;
using System.Text.Json;
using System.Threading;

namespace AzurePlayground.DocumentProcessor.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly QueueClient _queueClient;
    private readonly DocumentQueueProcessor _processor;
    private const int _maxAttempts = 3;

    public Worker(ILogger<Worker> logger, [FromKeyedServices("document-processing")] QueueClient queueClient, DocumentQueueProcessor processor)
    {
        _logger = logger;
        _queueClient = queueClient;
        _processor = processor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _queueClient.ReceiveMessageAsync(visibilityTimeout: TimeSpan.FromSeconds(10), cancellationToken: stoppingToken);            

            if (response.Value is not null)
            {
                await _processor.ProcessMessageAsync(response, stoppingToken);
            }            

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }    
}