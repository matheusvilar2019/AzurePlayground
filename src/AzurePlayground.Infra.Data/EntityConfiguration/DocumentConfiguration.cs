using AzurePlayground.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Infra.Data.EntityConfiguration
{
    internal class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.OriginalFileName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.BlobName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Container).HasMaxLength(100).IsRequired();
            builder.Property(e => e.ContentType).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Size).IsRequired();
            builder.Property(e => e.UploadedAt).IsRequired();
            builder.Property(e => e.Status).HasMaxLength(10).IsRequired();
        }
    }
}
