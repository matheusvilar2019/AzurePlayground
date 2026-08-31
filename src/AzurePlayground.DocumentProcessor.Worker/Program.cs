using Azure.Storage.Queues;
using AzurePlayground.DocumentProcessor.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var connectionString =
    builder.Configuration["AzureStorage:ConnectionString"];

var queueName =
    builder.Configuration["AzureStorage:QueueName"];

var queueClient = new QueueClient(
        connectionString,
        queueName);

await queueClient.CreateIfNotExistsAsync();

builder.Services.AddSingleton(queueClient);

var host = builder.Build();
host.Run();
