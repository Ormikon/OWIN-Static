using Ormikon.Owin.Static.Responses;
using System;

namespace Ormikon.Owin.Static.Cache
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
        /// Saves response data into the cache storage
        /// </summary>
        /// <param name="path">Requested HTTP path (Cache Key)</param>
        /// <param name="response">Cached response object</param>
        /// <param name="absoluteExpiration">Absolute expiration of the cached object. Never by default.</param>
        void Set(string path, CachedResponse response, DateTimeOffset? absoluteExpiration);
    }
}
