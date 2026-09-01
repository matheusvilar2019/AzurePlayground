using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using AzurePlayground.Application.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Text.Json;

namespace AzurePlayground.DocumentProcessor.Worker.Tests.Unit;

public class DocumentQueueProcessorTests
{
    [Fact]
    public async Task ProcessMessageAsync_WhenProcessingSucceeds_ShouldDeleteMessage()
    {
        // Arrange
        var queueClientMock = new Mock<QueueClient>();
        var poisonQueueClientMock = new Mock<QueueClient>();
        var loggerMock = new Mock<ILogger<DocumentQueueProcessor>>();

        var document = new DocumentProcessingMessageDTO
        {
            DocumentId = 1,
            FileName = "documento.pdf",
            ContentType = "application/pdf"
        };

        var message = QueuesModelFactory.QueueMessage(
            messageId: "message-1",
            popReceipt: "receipt-1",
            body: BinaryData.FromString(JsonSerializer.Serialize(document)),
            dequeueCount: 1);

        var processor = new TestableDocumentQueueProcessor(
            queueClientMock.Object,
            poisonQueueClientMock.Object,
            loggerMock.Object);

        // Act
        await processor.ProcessMessageAsync(message, CancellationToken.None);

        // Assert
        queueClientMock.Verify(
            x => x.DeleteMessageAsync(
                "message-1",
                "receipt-1",
                It.IsAny<CancellationToken>()),
            Times.Once);

        poisonQueueClientMock.Verify(
            x => x.SendMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenProcessingFails_ShouldNotDeleteMessage()
    {
        // Arrange
        var queueClientMock = new Mock<QueueClient>();
        var poisonQueueClientMock = new Mock<QueueClient>();
        var loggerMock = new Mock<ILogger<DocumentQueueProcessor>>();

        var document = new DocumentProcessingMessageDTO
        {
            DocumentId = 1,
            FileName = "documento.pdf",
            ContentType = "application/pdf"
        };

        var message = QueuesModelFactory.QueueMessage(
            messageId: "message-1",
            popReceipt: "receipt-1",
            body: BinaryData.FromString(JsonSerializer.Serialize(document)),
            dequeueCount: 1);

        var processor = new FailingDocumentQueueProcessor(
            queueClientMock.Object,
            poisonQueueClientMock.Object,
            loggerMock.Object);

        // Act
        await processor.ProcessMessageAsync(message, CancellationToken.None);

        // Assert
        queueClientMock.Verify(
            x => x.DeleteMessageAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        poisonQueueClientMock.Verify(
            x => x.SendMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenMaxAttemptsReached_ShouldMoveMessageToPoisonQueue()
    {
        // Arrange
        var queueClientMock = new Mock<QueueClient>();
        var poisonQueueClientMock = new Mock<QueueClient>();
        var loggerMock = new Mock<ILogger<DocumentQueueProcessor>>();

        var message = QueuesModelFactory.QueueMessage(
            messageId: "message-1",
            popReceipt: "receipt-1",
            body: BinaryData.FromString(
                """{"documentId":"123","fileName":"erro.pdf"}"""),
            dequeueCount: 3);

        var processor = new TestableDocumentQueueProcessor(
            queueClientMock.Object,
            poisonQueueClientMock.Object,
            loggerMock.Object);

        // Act
        await processor.ProcessMessageAsync(message, CancellationToken.None);

        // Assert
        poisonQueueClientMock.Verify(
            x => x.SendMessageAsync(
                message.Body.ToString(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        queueClientMock.Verify(
            x => x.DeleteMessageAsync(
                "message-1",
                "receipt-1",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenMessageIsInvalid_ShouldNotDeleteMessage()
    {
        // Arrange
        var queueClientMock = new Mock<QueueClient>();
        var poisonQueueClientMock = new Mock<QueueClient>();
        var loggerMock = new Mock<ILogger<DocumentQueueProcessor>>();

        var message = QueuesModelFactory.QueueMessage(
            messageId: "message-1",
            popReceipt: "receipt-1",
            body: BinaryData.FromString(
                "isto-nao-e-um-json-valido"),
            dequeueCount: 1);

        var processor = new TestableDocumentQueueProcessor(
            queueClientMock.Object,
            poisonQueueClientMock.Object,
            loggerMock.Object);

        // Act
        await processor.ProcessMessageAsync(message, CancellationToken.None);

        // Assert
        queueClientMock.Verify(
            x => x.DeleteMessageAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}