using Azure.Storage.Queues;
using AzurePlayground.DocumentProcessor.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var connectionString = builder.Configuration["AzureStorage:ConnectionString"];

var queueName = builder.Configuration["AzureStorage:QueueName"];
var poisonQueueName = builder.Configuration["AzureStorage:PoisonQueueName"];

var queueClient = new QueueClient(connectionString, queueName);
var poisonQueueClient = new QueueClient(connectionString, poisonQueueName);

await queueClient.CreateIfNotExistsAsync();
await poisonQueueClient.CreateIfNotExistsAsync();

builder.Services.AddSingleton<DocumentQueueProcessor>();
builder.Services.AddKeyedSingleton("document-processing", queueClient);
builder.Services.AddKeyedSingleton("document-processing-poison", poisonQueueClient);

var host = builder.Build();
host.Run();
