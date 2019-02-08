using System;
using System.Threading;
using System.Threading.Tasks;
using Ormikon.AspNetCore.Static.Responses;

namespace Ormikon.AspNetCore.Static.Cache
{
    /// <summary>
    /// Basic interface for caching Static files
    /// </summary>
    public interface IStaticCache
    {
        /// <summary>
        /// Gets cached response from the cache
        /// </summary>
        /// <param name="path">Requested HTTP path (Cache Key)</param>
        /// <returns>Found cache entity or null</returns>
        CachedResponse Get(string path);

        /// <summary>
        /// Gets cached response from the cache
        /// </summary>
        /// <param name="path">Requested HTTP path (Cache Key)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Found cache entity or null</returns>
        Task<CachedResponse> GetAsync(string path, CancellationToken cancellationToken);

        /// <summary>
        /// Saves response data into the cache storage
        /// </summary>
        /// <param name="path">Requested HTTP path (Cache Key)</param>
        /// <param name="response">Cached response object</param>
        /// <param name="absoluteExpiration">Absolute expiration of the cached object. Never by default.</param>
        void Set(string path, CachedResponse response, DateTimeOffset? absoluteExpiration);

        /// <summary>
        /// Saves response data into the cache storage
        /// </summary>
        /// <param name="path">Requested HTTP path (Cache Key)</param>
        /// <param name="response">Cached response object</param>
        /// <param name="absoluteExpiration">Absolute expiration of the cached object. Never by default.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        Task SetAsync(string path, CachedResponse response, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken);
    }
}
