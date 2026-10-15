using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace OnlineCinema.Logic.Services
{
    public static class BlobCleaner
    {
        public static async Task DeletePrefixAsync(BlobContainerClient container, string prefix, CancellationToken ct)
        {
            if (!await container.ExistsAsync(ct)) return;

            var batch = new List<BlobClient>(64);

            await foreach (var blobItem in container.GetBlobsAsync(
                BlobTraits.None, BlobStates.None, prefix, ct))
            {
                batch.Add(container.GetBlobClient(blobItem.Name));

                if (batch.Count == 64)
                {
                    await Task.WhenAll(batch.Select(b => b.DeleteIfExistsAsync()));
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
                await Task.WhenAll(batch.Select(b => b.DeleteIfExistsAsync()));
        }
    }
}
