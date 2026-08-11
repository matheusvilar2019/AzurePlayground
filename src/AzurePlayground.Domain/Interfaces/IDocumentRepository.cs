using AzurePlayground.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Domain.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetDocumentsAsync();
        Task<Document> GetByIdAsync(int? id);

        Task<Document> CreateAsync(Document document);
        Task<Document> UpdateAsync(Document document);
        Task<Document> RemoveAsync(Document document);
    }
}
