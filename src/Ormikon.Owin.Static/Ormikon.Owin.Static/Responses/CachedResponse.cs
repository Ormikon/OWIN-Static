using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Wrappers.Headers;
using System.Collections.Generic;

namespace Ormikon.Owin.Static.Responses
{
    /// <summary>
    /// Cached static entity response model.
    /// </summary>
    [Serializable]
    public class CachedResponse : IStaticResponse
    {
        private const int CopyBufferSize = 32768;

        private readonly IHttpResponseHeaders headers;

        /// <summary>
        /// Creates an empty cached response object
        /// </summary>
        public CachedResponse()
        {
            headers = new HttpResponseHeaders();
        }

        internal static Task<CachedResponse> CreateAsync(StaticResponse staticResponse, CancellationToken cancellationToken)
        {
            var mem = new MemoryStream();
            var stream = staticResponse.Body;
            return stream.CopyToAsync(mem, CopyBufferSize, cancellationToken)
                         .ContinueWith(
                             task =>
                                 {
                                     stream.Close();
                                     task.Wait(cancellationToken);
                                     var result = new CachedResponse
                                                      {
                                                          StatusCode = staticResponse.StatusCode,
                                                          Body = mem.ToArray()
                                                      };
                                     staticResponse.Headers.CopyTo(result.headers);
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

        internal Stream CreateBodyStream()
        {
            return new MemoryStream(Body, false);
        }

        /// <summary>
        /// HTTP Status code of the cached response
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// HTTP Status reason of the cached response
        /// </summary>
        public string ReasonPhrase { get; set; }

        IHttpResponseHeaders IStaticResponse.Headers {
            get
            {
                return headers;
            }
        }

        /// <summary>
        /// HTTP Response headers collection
        /// </summary>
        public IDictionary<string, string[]> Headers
        {
            get
            {
                return headers;
            }
        }

        /// <summary>
        /// Response body data encoded
        /// </summary>
        public byte[] Body { get; set; }
    }
}
