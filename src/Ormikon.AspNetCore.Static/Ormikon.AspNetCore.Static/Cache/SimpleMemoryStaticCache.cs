using System;
using System.Collections.Concurrent;
using Ormikon.AspNetCore.Static.Responses;

namespace Ormikon.AspNetCore.Static.Cache
{
    /// <summary>
    /// Simple In memory implementation of a response static cache
    /// </summary>
    internal class SimpleMemoryStaticCache : IStaticCache
    {
        private readonly ConcurrentDictionary<string, Tuple<DateTimeOffset, CachedResponse>> cache =
            new ConcurrentDictionary<string, Tuple<DateTimeOffset, CachedResponse>>(StringComparer.OrdinalIgnoreCase);

        public CachedResponse Get(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            Tuple<DateTimeOffset, CachedResponse> crt;
            if (cache.TryGetValue(path, out crt))
            {
                if (crt.Item1 < DateTimeOffset.Now)
                {
                    cache.TryRemove(path, out crt);
                    return null;
                }
                return crt.Item2;
            }
            return null;
        }

        public void Set(string path, CachedResponse response, DateTimeOffset? absoluteExpiration)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            cache.TryAdd(path, Tuple.Create(absoluteExpiration ?? DateTimeOffset.MaxValue, response));
        }
    }
}
