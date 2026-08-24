using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AzurePlayground.Api.HealthChecks
{
    public class AzureBlobHealthCheck : IHealthCheck
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobHealthCheck(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _containerClient.ExistsAsync(cancellationToken);

                return HealthCheckResult.Healthy(
                    "Azure Blob Storage is available.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Azure Blob Storage is unavailable.",
                    ex);
            }
        }
    }
}
