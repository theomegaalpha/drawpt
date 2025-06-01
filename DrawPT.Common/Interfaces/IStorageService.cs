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
        Task SaveImageAsync(string imageId, string imageUrl);
    }
} 