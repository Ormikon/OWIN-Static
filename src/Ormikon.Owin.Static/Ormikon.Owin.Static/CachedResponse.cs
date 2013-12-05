using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static
{
    [Serializable]
    internal class CachedResponse : IResponseHeaders
    {
        public CachedResponse()
        {
            Headers = new Dictionary<string, string[]>();
        }

        public static Task<CachedResponse> CreateAsync(StaticResponse staticResponse)
        {
            var mem = new MemoryStream();
            return staticResponse.Body.CopyToAsync(mem)
                                 .ContinueWith(
                                     task =>
                                         {
                                             task.Wait();
                                             var result = new CachedResponse
                                                              {
                                                                  StatusCode = staticResponse.StatusCode,
                                                                  Body = mem.ToArray()
                                                              };
                                             staticResponse.Headers.CopyTo(result.Headers);
                                             return result;
                                         }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public Stream CreateBodyStream()
        {
            return new MemoryStream(Body);
        }

        public int StatusCode { get; set; }

        public IDictionary<string, string[]> Headers { get; private set; }

        public byte[] Body { get; set; }
    }
}
