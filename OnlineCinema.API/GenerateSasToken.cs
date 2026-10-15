using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace OnlineCinema.API
{
    public static class GenerateSasToken
    {
        public static string GetSasUri(string connectionString, string containerName)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Resource = "c",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(12)
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
            return sasUri.Query;
        }
    }
}
