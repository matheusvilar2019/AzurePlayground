using AzurePlayground.Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzurePlayground.Domain.Tests
{
    public class DocumentUnitTest1
    {
        [Fact]
        public void CreateDocument_WithValidParameters_ResultObjectValidState()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", "Container", "ContType",
                100000, "Status");
            action.Should()
                .NotThrow<AzurePlayground.Domain.Validation.DomainExceptionValidation>();
        }

        [Fact]
        public void CreateDocument_NegativeIdValue_DomainExceptionInvalidId()
        {
            Action action = () => new Document(-1, "Original File Name", "Blob Name", "Container", "ContType",
                100000, "Status");

            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid Id value");
        }

        [Fact]
        public void CreateDocument_WithNullOriginalFileName_DomainExceptionRequired()
        {
            Action action = () => new Document(1, null, "Blob Name", "Container", "ContType",
                100000, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid original file name. Original file name is required");
        }

        [Fact]
        public void CreateDocument_ShortOriginalFileNameValue_DomainExceptionShortName()
        {
            Action action = () => new Document(1, "Or", "Blob Name", "Container", "ContType",
                100000, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid original file name, too short, minimum 3 characters");
        }

        [Fact]
        public void CreateDocument_WithNullBlobName_DomainExceptionRequired()
        {
            Action action = () => new Document(1, "Original File Name", null, "Container", "ContType",
                100000, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid blobName. BlobName is required");
        }

        [Fact]
        public void CreateDocument_ShortBlobNameValue_DomainExceptionShortName()
        {
            Action action = () => new Document(1, "Original File Name", "Bl", "Container", "ContType",
                100000, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid blobName, too short, minimum 3 characters");
        }

        [Fact]
        public void CreateDocument_WithNullContainer_DomainExceptionRequired()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", null, "ContType",
                100000, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid container. Container is required");
        }

        [Fact]
        public void CreateDocument_ShortContainerValue_DomainExceptionShortName()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", "Co", "ContType",
                100000, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid container, too short, minimum 3 characters");
        }

        [Fact]
        public void CreateDocument_WithNullContentType_DomainExceptionRequired()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", "Container", null,
                100000, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid contentType. ContentType is required");
        }

        [Fact]
        public void CreateDocument_ShortContentTypeValue_DomainExceptionShortName()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", "Container", "Co",
                100000, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid contentType, too short, minimum 3 characters");
        }

        [Fact]
        public void CreateDocument_InvalidSizeValue_DomainException()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", "Container", "ContType",
                -1, "Status");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid size value");
        }

        [Fact]
        public void CreateDocument_WithNullStatus_DomainExceptionRequired()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", "Container", "ContType",
                100000, null);
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid status. Status is required");
        }

        [Fact]
        public void CreateDocument_ShortStatusValue_DomainExceptionShortName()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", "Container", "ContType",
                100000, "St");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid status, too short, minimum 3 characters");
        }

        [Fact]
        public void CreateDocument_LongImageName_DomainExceptionLongImageName()
        {
            Action action = () => new Document(1, "Original File Name", "Blob Name", "Container", "ContType",
                100000, "Status long");
            action.Should().Throw<AzurePlayground.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid status, too long, maximum 10 characters");
        }
    }
}
