using AzurePlayground.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDTO>> GetDocuments();
        Task<DocumentDTO> GetById(int? id);
        Task Add(DocumentDTO documentDTO);
        Task Update(DocumentDTO documentDTO);
        Task Remove(int? id);
    }
}
