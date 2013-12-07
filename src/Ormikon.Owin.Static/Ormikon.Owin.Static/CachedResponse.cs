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
            var stream = staticResponse.Body;
            return stream.CopyToAsync(mem)
                         .ContinueWith(
                             task =>
                                 {
                                     stream.Close();
                                     task.Wait();
                                     var result = new CachedResponse
                                                      {
                                                          StatusCode = staticResponse.StatusCode,
                                                          Body = mem.ToArray()
                                                      };
                                     PreprocessHeaders(staticResponse, result.Body.Length);
                                     staticResponse.Headers.CopyTo(result.Headers);
                                     return result;
                                 }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private static void PreprocessHeaders(StaticResponse staticResponse, int contentLength)
        {
            staticResponse.ContentLength = contentLength;
            if (!staticResponse.LastModified.HasValue)
                staticResponse.LastModified = DateTimeOffset.Now;
            if (!staticResponse.Date.HasValue)
                staticResponse.Date = DateTimeOffset.Now;
            if (staticResponse.ETag == null)
                staticResponse.ETag = Guid.NewGuid().ToString("N");
            int? maxAge = staticResponse.MaxAge;
            if (maxAge.HasValue && !staticResponse.Expires.HasValue)
                staticResponse.Expires = DateTimeOffset.Now.AddSeconds(maxAge.Value);
        }

        public Stream CreateBodyStream()
        {
            return new MemoryStream(Body);
        }

        public int StatusCode { get; set; }

        public IDictionary<string, string[]> Headers { get; private set; }

        public string ETag
        {
            get { return Headers.GetSingleValue(Constants.Http.Headers.ETag); }
        }

        public byte[] Body { get; set; }
    }
}
