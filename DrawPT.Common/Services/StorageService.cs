using Azure.Identity;
using Azure.Storage.Blobs;

using DrawPT.Common.Interfaces;

namespace DrawPT.Common.Services
{
    public class StorageService : IStorageService
    {
        private readonly string _storageContainerName = "images";
        private readonly BlobServiceClient _blobServiceClient;

        public StorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string?> SaveImageAsync(byte[] imageBytes, string blobName)
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

        public async Task<byte[]> GetImageAsync(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "Blob name cannot be null or empty.");
            }

            try
            {
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_storageContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                if (await blobClient.ExistsAsync())
                {
                    using var memoryStream = new MemoryStream();
                    await blobClient.DownloadToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
                else
                {
                    Console.WriteLine($"Blob {blobName} does not exist.");
                    return Array.Empty<byte>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve image ({blobName}) from blob storage: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        public async Task<string?> DownloadImageAsync(string imageUrl, string blobName)
        {
            try
            {
                Uri uri = new Uri(imageUrl);
                string extension = Path.GetExtension(uri.AbsolutePath);

                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_storageContainerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                using var imageStream = await response.Content.ReadAsStreamAsync();

                var client = await blobClient.UploadAsync(imageStream, true);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save image ({blobName}) to blob storage.", ex.Message);
                return null;
            }
        }
    }
}
