using System.Threading.Tasks;

namespace DrawPT.Common.Interfaces
{
    /// <summary>
    /// Interface for storage-related services
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Saves an image to storage
        /// </summary>
        Task<string?> SaveImageAsync(byte[] imageBytes, string blobName);

        Task<bool> DownloadImageAsync(Guid id, string imageUrl);
    }
}