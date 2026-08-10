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
        Task<IEnumerable<Document>> GetDocuments();
        Task<Document> GetById(int? id);

        Task<Document> Create(Document document);
        Task<Document> Update(Document document);
        Task<Document> Delete(Document document);
    }
}
