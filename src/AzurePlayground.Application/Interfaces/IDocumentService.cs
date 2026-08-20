using AzurePlayground.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentListDTO>> GetDocuments();
        Task<DocumentListDTO> GetById(int id);
        Task<DocumentDTO> Add(DocumentUploadDTO document);
        Task<DocumentDownloadDTO?> Download(int id);
        Task Update(DocumentDTO documentDTO);
        Task<bool> Remove(int id);
    }
}
