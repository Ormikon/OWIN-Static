using System;
using System.IO;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.Responses
{
    [Serializable]
    internal class CachedResponse : IStaticResponse
    {
        public CachedResponse()
        {
            Headers = new HttpResponseHeaders();
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
                                     staticResponse.Headers.CopyTo(result.Headers);
                                     PostProcessHeaders(result, result.Body.Length);
                                     return result;
                                 }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private static void PostProcessHeaders(IStaticResponse response, int contentLength)
        {
            if (response.StatusCode != Constants.Http.StatusCodes.Successful.Ok)
                return;
            response.Headers.ContentLength.Value = contentLength;
            if (!response.Headers.LastModified.Available)
                response.Headers.LastModified.Value = DateTimeOffset.Now;
            if (!response.Headers.Date.Available)
                response.Headers.Date.Value = DateTimeOffset.Now;
            if (!response.Headers.ETag.Available)
                response.Headers.ETag.Value = Guid.NewGuid().ToString("N");
            int? maxAge = response.Headers.CacheControl.MaxAge;
            if (maxAge.HasValue && !response.Headers.Expires.Available)
                response.Headers.Expires.Value = DateTimeOffset.Now.AddSeconds(maxAge.Value);
        }

        public Stream CreateBodyStream()
        {
            return new MemoryStream(Body, false);
        }

        public int StatusCode { get; set; }

        public string ReasonPhrase { get; set; }

        public IHttpResponseHeaders Headers { get; private set; }

        public byte[] Body { get; set; }
    }
}
