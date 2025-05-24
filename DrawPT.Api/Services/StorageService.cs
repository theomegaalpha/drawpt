using Azure.Identity;
using Azure.Storage.Blobs;

namespace DrawPT.Api.Services
{
    public class StorageService
    {
        private readonly string _storageContainerName = "images";

        public async Task<bool> DownloadImage(Guid id, string imageUrl)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(
                        new Uri("https://drawptstorageacct.blob.core.windows.net"),
                        new DefaultAzureCredential());

                Uri uri = new Uri(imageUrl);
                string extension = Path.GetExtension(uri.AbsolutePath);

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_storageContainerName);
                var blobClient = containerClient.GetBlobClient($"{id}{extension}");

                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                using var imageStream = await response.Content.ReadAsStreamAsync();

                //await blobClient.UploadAsync(imageStream, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save image ({id}) to blob storage.", ex.Message);
                return false;
            }

            return true;
        }
    }
}
