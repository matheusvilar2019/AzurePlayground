using AutoMapper;
using AzurePlayground.Application.DTOs;
using AzurePlayground.Application.Interfaces;
using AzurePlayground.Application.Validators;
using AzurePlayground.Domain.Entities;
using AzurePlayground.Domain.Interfaces;
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

        public DocumentService(IMapper mapper, IDocumentRepository documentRepository, IDocumentStorage documentStorage)
        {
            _documentRepository = documentRepository ??
                throw new ArgumentNullException(nameof(documentRepository));
            _documentStorage = documentStorage;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DocumentDTO>> GetDocuments()
        {
            var documentsEntity = await _documentRepository.GetDocumentsAsync();
            return _mapper.Map<IEnumerable<DocumentDTO>>(documentsEntity);
        }

        public async Task<DocumentDTO> GetById(int? id)
        {
            var documentEntity = await _documentRepository.GetByIdAsync(id);
            return _mapper.Map<DocumentDTO>(documentEntity);
        }

        public async Task<DocumentDTO> Add(DocumentUploadDTO documentUploadDTO)
        {
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

        public async Task Update(DocumentDTO documentDTO)
        {
            var documentEntity = _mapper.Map<Document>(documentDTO);
            await _documentRepository.UpdateAsync(documentEntity);
        }

        public async Task Remove(int? id)
        {
            var documentEntity = _documentRepository.GetByIdAsync(id).Result;
            await _documentRepository.RemoveAsync(documentEntity);
        }
    }
}
