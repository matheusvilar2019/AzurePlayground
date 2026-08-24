using AutoMapper;
using AzurePlayground.Application.DTOs;
using AzurePlayground.Application.Interfaces;
using AzurePlayground.Application.Validators;
using AzurePlayground.Domain.Entities;
using AzurePlayground.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private IDocumentRepository _documentRepository;
        private readonly IDocumentStorage _documentStorage;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IMapper mapper, IDocumentRepository documentRepository, IDocumentStorage documentStorage, ILogger<DocumentService> logger)
        {
            _documentRepository = documentRepository
                ?? throw new ArgumentNullException(nameof(documentRepository));

            _documentStorage = documentStorage
                ?? throw new ArgumentNullException(nameof(documentStorage));

            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));

            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<DocumentListDTO>> GetDocuments()
        {
            var documentsEntity = await _documentRepository.GetDocumentsAsync();
            return _mapper.Map<IEnumerable<DocumentListDTO>>(documentsEntity);
        }

        public async Task<DocumentListDTO> GetById(int id)
        {
            var documentEntity = await _documentRepository.GetByIdAsync(id);
            return _mapper.Map<DocumentListDTO>(documentEntity);
        }

        public async Task<DocumentAdminDTO> GetAdminInfo(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);

            if (document == null)
                return new DocumentAdminDTO();

            var storageInfo = await _documentStorage.GetInfoAsync(
                document.Container,
                document.BlobName);

            return new DocumentAdminDTO
            {
                Id = document.Id,
                OriginalFileName = document.OriginalFileName,
                BlobName = document.BlobName,
                Container = document.Container,
                UploadedAt = document.UploadedAt,
                Storage = storageInfo
            };
        }

        public async Task<DocumentDTO> Add(DocumentUploadDTO documentUploadDTO)
        {
            _logger.LogInformation(
                "Starting document upload. FileName: {FileName}, ContentType: {ContentType}, Size: {Size}",
                documentUploadDTO.OriginalFileName,
                documentUploadDTO.ContentType,
                documentUploadDTO.Content.Length);

            DocumentValidator.Validate(documentUploadDTO);

            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(documentUploadDTO.OriginalFileName)}";

            var container = "documents";

            var size = documentUploadDTO.Content.Length;

            var document = new Document(
                documentUploadDTO.OriginalFileName,
                blobName,
                container,
                documentUploadDTO.ContentType,
                (int)size,
                "Active");

            await _documentStorage.UploadAsync(
                documentUploadDTO.Content,
                blobName,
                documentUploadDTO.ContentType);

            _logger.LogInformation(
                "Document uploaded to storage successfully. BlobName: {BlobName}",
                document.BlobName);

            try
            {
                var documentEntity = await _documentRepository.CreateAsync(document);

                return _mapper.Map<DocumentDTO>(documentEntity);
            }
            catch
            {
                await _documentStorage.DeleteAsync(blobName);
                throw;
            }
        }

        public async Task<DocumentDownloadDTO?> Download(int id)
        {
            _logger.LogInformation("Starting document download. DocumentId: {DocumentId}", id);
            var documentEntity = await _documentRepository.GetByIdAsync(id);

            if (documentEntity == null)
            {
                _logger.LogWarning("Document not found for download. DocumentId: {DocumentId}", id);
                return null;
            }

            var file = await _documentStorage.DownloadAsync(documentEntity.BlobName);

            _logger.LogInformation(
                "Document downloaded successfully. DocumentId: {DocumentId}, BlobName: {BlobName}",
                id,
                documentEntity.BlobName);

            return new DocumentDownloadDTO
            {
                Content = file,
                ContentType = documentEntity.ContentType,
                FileName = documentEntity.OriginalFileName
            };
        }

        public async Task Update(DocumentDTO documentDTO)
        {
            var documentEntity = _mapper.Map<Document>(documentDTO);
            await _documentRepository.UpdateAsync(documentEntity);
        }

        public async Task<bool> Remove(int id)
        {
            _logger.LogInformation("Starting document removal. DocumentId: {DocumentId}", id);

            var document = await _documentRepository.GetByIdAsync(id);

            if (document == null)
            {
                _logger.LogWarning("Document not found for removal. DocumentId: {DocumentId}", id);
                return false;
            }

            await _documentStorage.DeleteAsync(document.BlobName);

            _logger.LogInformation(
                "Document blob removed successfully. DocumentId: {DocumentId}, BlobName: {BlobName}",
                id,
                document.BlobName);

            await _documentRepository.RemoveAsync(document);

            _logger.LogInformation(
                "Document metadata removed successfully. DocumentId: {DocumentId}, BlobName: {BlobName}",
                id,
                document.BlobName);

            return true;
        }
    }
}
