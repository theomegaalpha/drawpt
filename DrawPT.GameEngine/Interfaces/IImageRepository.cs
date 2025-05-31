using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Interface for image repository operations
    /// </summary>
    public interface IImageRepository
    {
        /// <summary>
        /// Caches an image's information
        /// </summary>
        Task CacheImageAsync(CachedImage image);
    }
} 