using AzurePlayground.Domain.Entities;
using AzurePlayground.Domain.Interfaces;
using AzurePlayground.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Infra.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private ApplicationDbContext _context;
        public DocumentRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<Document> Create(Document document)
        {
            _context.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document> GetById(int? id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<IEnumerable<Document>> GetDocuments()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<Document> Remove(Document document)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document> Update(Document document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }
    }
}
