using Azure.Storage.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.DocumentProcessor.Worker.Tests.Integration;

public abstract class IntegrationTestBase
{
    protected const string ConnectionString = "UseDevelopmentStorage=true";

    protected QueueClient CreateQueueClient(string queueName)
    {
        return new QueueClient(ConnectionString, queueName);
    }

    protected static string CreateQueueName()
    {
        return $"test-{Guid.NewGuid():N}";
    }
}