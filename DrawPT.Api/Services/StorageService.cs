using Azure.Identity;
using Azure.Storage.Blobs;

namespace DrawPT.Api.Services
{
    public class StorageService
    {
        private readonly string _storageContainerName = "images";
        private readonly BlobServiceClient _blobServiceClient;

        public StorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string?> UploadImageAsync(byte[] imageBytes, string blobName)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(imageBytes), "Image bytes cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "Blob name cannot be null or empty.");
            }

            try
            {
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_storageContainerName);
                await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob); // Ensure container exists and is public for URLs

                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                using var stream = new MemoryStream(imageBytes);
                await blobClient.UploadAsync(stream, overwrite: true);

                Console.WriteLine($"Image {blobName} uploaded to blob storage.");
                return blobClient.Uri.ToString(); // Return the public URL
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload image ({blobName}) to blob storage: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DownloadImage(Guid id, string imageUrl)
        {
            try
            {
                Uri uri = new Uri(imageUrl);
                string extension = Path.GetExtension(uri.AbsolutePath);

                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_storageContainerName);
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
