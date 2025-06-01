using DrawPT.Common.Interfaces;

namespace DrawPT.GameEngine.Services
{
    /// <summary>
    /// Interface for storage-related services
    /// </summary>
    public class StorageService : IStorageService
    {
        /// <summary>
        /// Saves an image to storage
        /// </summary>
        public Task SaveImageAsync(string imageId, string imageUrl)
        {
            return Task.CompletedTask;
        }
    }
} 