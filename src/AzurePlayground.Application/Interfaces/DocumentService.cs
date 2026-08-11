using AutoMapper;
using AzurePlayground.Application.DTOs;
using AzurePlayground.Application.Services;
using AzurePlayground.Domain.Entities;
using AzurePlayground.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Interfaces
{
    public class DocumentService : IDocumentService
    {
        private IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        public DocumentService(IMapper mapper, IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository ??
                throw new ArgumentNullException(nameof(documentRepository));

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

        public async Task Add(DocumentDTO documentDTO)
        {
            var documentEntity = _mapper.Map<Document>(documentDTO);
            await _documentRepository.CreateAsync(documentEntity);
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
