using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzurePlayground.DocumentProcessor.Worker.Tests.Integration;

public class QueueIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task SendMessageAsync_ShouldBeReceived()
    {
        // Arrange
        var queueName = CreateQueueName();

        var queueClient = CreateQueueClient(queueName);

        await queueClient.CreateIfNotExistsAsync();

        const string messageBody = "mensagem de teste";

        try
        {
            // Act
            await queueClient.SendMessageAsync(messageBody);

            QueueMessage[] messages =
                await queueClient.ReceiveMessagesAsync(
                    maxMessages: 1);

            // Assert
            var message = Assert.Single(messages);

            Assert.Equal(
                messageBody,
                message.Body.ToString());
        }
        finally
        {
            await queueClient.DeleteIfExistsAsync();
        }
    }

    [Fact]
    public async Task DeleteMessageAsync_ShouldRemoveMessage()
    {
        // Arrange
        var queueName = CreateQueueName();

        var queueClient = CreateQueueClient(queueName);

        await queueClient.CreateIfNotExistsAsync();

        try
        {
            await queueClient.SendMessageAsync(
                "mensagem de teste");

            var messages =
                await queueClient.ReceiveMessagesAsync(
                    maxMessages: 1);

            var message = Assert.Single(messages.Value);

            // Act
            await queueClient.DeleteMessageAsync(
                message.MessageId,
                message.PopReceipt);

            // Assert
            var messagesAfterDelete =
                await queueClient.ReceiveMessagesAsync(
                    maxMessages: 1);

            Assert.Empty(messagesAfterDelete.Value);
        }
        finally
        {
            await queueClient.DeleteIfExistsAsync();
        }
    }

    [Fact]
    public async Task Message_ShouldBecomeVisibleAgain_AfterVisibilityTimeout()
    {
        // Arrange
        var queueName = CreateQueueName();

        var queueClient = CreateQueueClient(queueName);

        await queueClient.CreateIfNotExistsAsync();

        try
        {
            await queueClient.SendMessageAsync(
                "mensagem de retry");

            // Primeiro Receive
            var firstReceive =
                await queueClient.ReceiveMessagesAsync(
                    maxMessages: 1,
                    visibilityTimeout: TimeSpan.FromSeconds(2));

            var firstMessage = Assert.Single(firstReceive.Value);

            Assert.Equal(1, firstMessage.DequeueCount);

            // Imediatamente depois:
            // mensagem está invisível.
            var immediateReceive =
                await queueClient.ReceiveMessagesAsync(
                    maxMessages: 1);

            Assert.Empty(immediateReceive.Value);

            // Aguarda o Visibility Timeout.
            await Task.Delay(
                TimeSpan.FromSeconds(3));

            // Segundo Receive
            var secondReceive =
                await queueClient.ReceiveMessagesAsync(
                    maxMessages: 1);

            var secondMessage =
                Assert.Single(secondReceive.Value);

            // Assert
            Assert.Equal(
                2,
                secondMessage.DequeueCount);
        }
        finally
        {
            await queueClient.DeleteIfExistsAsync();
        }
    }

    [Fact]
    public async Task Message_ShouldBeMovedToPoisonQueue()
    {
        // Arrange
        var queueName = CreateQueueName();
        var poisonQueueName = CreateQueueName();

        var queueClient =
            CreateQueueClient(queueName);

        var poisonQueueClient =
            CreateQueueClient(poisonQueueName);

        await queueClient.CreateIfNotExistsAsync();
        await poisonQueueClient.CreateIfNotExistsAsync();

        try
        {
            const string messageBody =
                "mensagem problemática";

            await queueClient.SendMessageAsync(
                messageBody);

            var messages =
                await queueClient.ReceiveMessagesAsync(
                    maxMessages: 1);

            var message = Assert.Single(messages.Value);

            // Simula que atingimos o limite.
            await poisonQueueClient.SendMessageAsync(
                message.Body.ToString());

            await queueClient.DeleteMessageAsync(
                message.MessageId,
                message.PopReceipt);

            // Assert - original não existe mais
            var originalMessages =
                await queueClient.ReceiveMessagesAsync(
                    maxMessages: 1);

            Assert.Empty(originalMessages.Value);

            // Assert - poison existe
            var poisonMessages =
                await poisonQueueClient.ReceiveMessagesAsync(
                    maxMessages: 1);

            var poisonMessage =
                Assert.Single(poisonMessages.Value);

            Assert.Equal(
                messageBody,
                poisonMessage.Body.ToString());
        }
        finally
        {
            await queueClient.DeleteIfExistsAsync();
            await poisonQueueClient.DeleteIfExistsAsync();
        }
    }
}