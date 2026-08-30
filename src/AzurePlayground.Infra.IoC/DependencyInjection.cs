using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using AzurePlayground.Application.Interfaces;
using AzurePlayground.Application.Mappings;
using AzurePlayground.Application.Services;
using AzurePlayground.Domain.Interfaces;
using AzurePlayground.Infra.Data.Context;
using AzurePlayground.Infra.Data.Queue;
using AzurePlayground.Infra.Data.Repositories;
using AzurePlayground.Infra.Data.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Infra.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString =
            configuration["AzureStorage:ConnectionString"]
            ?? throw new InvalidOperationException(
                "Azure Storage connection string was not configured.");

            var containerName =
                configuration["AzureStorage:ContainerName"]
                ?? throw new InvalidOperationException(
                    "Azure Storage container name was not configured.");

            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"
            ), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DomainToDTOMappingProfile>();
            });

            services.AddScoped<IDocumentStorage>(_ =>
            new AzureBlobDocumentStorage(
                connectionString,
                containerName));

            services.AddSingleton<BlobServiceClient>(serviceProvider =>
            {
                var configuration = serviceProvider
                    .GetRequiredService<IConfiguration>();

                var connectionString =
                    configuration["AzureStorage:ConnectionString"];

                return new BlobServiceClient(connectionString);
            });

            services.AddSingleton<BlobContainerClient>(serviceProvider =>
            {
                var blobServiceClient =
                    serviceProvider.GetRequiredService<BlobServiceClient>();

                var containerName =
                    configuration["AzureStorage:ContainerName"];

                return blobServiceClient.GetBlobContainerClient(containerName);
            });

            const string queueName = "document-processing";

            services.AddSingleton(
                new QueueClient(
                    connectionString,
                    queueName));

            services.AddScoped<IDocumentQueueService, AzureQueueDocumentService>();

            return services;
        }
    }
}
