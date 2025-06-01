using DrawPT.Common.Models;

namespace DrawPT.Common.Interfaces
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