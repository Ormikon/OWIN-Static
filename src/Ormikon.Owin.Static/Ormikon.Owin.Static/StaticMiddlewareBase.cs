using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;
using Ormikon.Owin.Static.Filters;
using System.IO.Compression;

namespace Ormikon.Owin.Static
{
    internal abstract class StaticMiddlewareBase : OwinMiddleware
    {
        private readonly bool cached;
        private readonly ObjectCache cache;
        private readonly IFilter compressedContentFilter;
        private readonly DateTimeOffset expires;
        private readonly int maxAge;

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next)
            : this(next, false, null, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next, bool cached)
            : this(next, cached, null, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next, bool cached, ObjectCache cache)
            : this(next, cached, cache, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next, bool cached, ObjectCache cache,
            string compressedContentFilter)
            : this(next, cached, cache, DateTimeOffset.MinValue, 0, compressedContentFilter)
        {
        }

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next, bool cached, ObjectCache cache,
            DateTimeOffset expires, int maxAge)
            : this(next, cached, cache, expires, maxAge, StaticSettings.DefaultCompressedTypesFilter)
        {
        }

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next, bool cached, ObjectCache cache,
            DateTimeOffset expires, int maxAge, string compressedContentFilter)
            : base(next)
        {
            this.cached = cached;
            this.cache = cache;
            this.expires = expires;
            this.maxAge = maxAge;
            this.compressedContentFilter = new ContentTypeFilter(compressedContentFilter);
        }

        protected override Task Invoke(IOwinContext context)
        {
            return IsMethodAllowed(context.Request.Method)
                    ? ProcessStaticIfFound(context) ?? Next(context)
                    : Next(context);
        }

        #region abstract methods

        protected abstract StaticResponse GetResponse(Location location);

        #endregion

        #region private methods

        private static Task SendStreamAsync(Stream from, Stream to)
        {
            return from.CopyToAsync(to)
                .ContinueWith(
                task =>
                {
                    from.Close();
                    task.Wait();
                    to.Close();
                }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private DateTimeOffset GetCacheOffset()
        {
            if (maxAge > 0)
                return DateTimeOffset.Now.AddSeconds(maxAge);
            if (expires != DateTimeOffset.MinValue && expires > DateTimeOffset.Now)
                return expires;
            return DateTimeOffset.MaxValue;// never expires
        }

        private CachedResponse CacheGet(Location location)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            return c.Get(location.FullPath) as CachedResponse;
        }

        private void CacheSet(string path, CachedResponse data)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            c.Set(path, data, GetCacheOffset());
        }

        private static void SetResponseHeaders(IStaticResponse staticResponse, IOwinResponse response)
        {
            response.StatusCode = staticResponse.StatusCode;
            if (!string.IsNullOrEmpty(staticResponse.ReasonPhrase))
                response.ReasonPhrase = staticResponse.ReasonPhrase;
            staticResponse.Headers.CopyTo(response.Headers);
        }

        private bool CanResponseBeCompressed(IStaticResponse responseHeaders, IOwinContext ctx, out string encoding)
        {
            encoding = null;
            if (!compressedContentFilter.IsActive() || responseHeaders.StatusCode != Constants.Http.StatusCodes.Successful.Ok)
                return false;
            var propValues = responseHeaders.Headers.ContentType.PropertyValues;
            if (propValues.Length == 0)
                return false;
            if (compressedContentFilter.Contains(propValues[0].Value))
            {
                encoding = "gzip";
                return true;
            }
            return false;
        }

        private static void PrepareResponseToCompression(IStaticResponse response, string encoding)
        {
            response.Headers.ContentLength.Clear();
            response.Headers.ContentEncoding.Value = encoding;
        }

        private static Stream WrapIntoCompressedStream(Stream stream, string encoding)
        {
            if (encoding == null)
                return stream;
            if (encoding == "gzip")
            {
                return new GZipStream(stream, CompressionMode.Compress, false);
            }
            if (encoding == "deflate")
            {
                return new DeflateStream(stream, CompressionMode.Compress, false);
            }
            return stream;
        }

        private Task ProcessResponseStream(IStaticResponse response, Stream stream, IOwinContext ctx)
        {
            if (response.StatusCode != Constants.Http.StatusCodes.Successful.Ok)
            {
                SetResponseHeaders(response, ctx.Response);
                return SendStreamAsync(stream, ctx.Response.Body);
            }
            string encoding;
            bool compress = CanResponseBeCompressed(response, ctx, out encoding);
            if (compress)
                PrepareResponseToCompression(response, encoding);
            SetResponseHeaders(response, ctx.Response);
            if (IsBodyRequested(ctx.Request.Method))
                return SendStreamAsync(stream, WrapIntoCompressedStream(ctx.Response.Body, encoding));
            stream.Close();
            return Task.FromResult<object>(null);
        }

        private Task SendResponse(CachedResponse cachedResponse, IOwinContext ctx)
        {
            return ProcessResponseStream(cachedResponse, cachedResponse.CreateBodyStream(), ctx);
        }

        private Task SendResponse(StaticResponse staticResponse, IOwinContext ctx)
        {
            return ProcessResponseStream(staticResponse, staticResponse.Body, ctx);
        }

        private Task CacheResponseAndSend(StaticResponse staticResponse, IOwinContext ctx)
        {
            if (cached)
            {
                return CachedResponse.CreateAsync(staticResponse)
                        .ContinueWith(
                            task =>
                            {
                                task.Wait();
                                CacheSet(ctx.Request.Location.FullPath, task.Result);
                                return SendResponse(task.Result, ctx);
                            }, TaskContinuationOptions.ExecuteSynchronously)
                        .Unwrap();
            }
            return SendResponse(staticResponse, ctx);
        }

        private Task ProcessStaticIfFound(IOwinContext ctx)
        {
            var location = ctx.Request.Location;
            if (cached)
            {
                var response = CacheGet(location);
                if (response != null)
                    return SendResponse(response, ctx);
            }

            var staticResponse = GetResponse(location);
            return staticResponse == null ? null : CacheResponseAndSend(staticResponse, ctx);
        }

        private static bool IsMethodAllowed(string method)
        {
            return string.Compare(method, Constants.Http.Methods.Get, StringComparison.OrdinalIgnoreCase) == 0
                   || string.Compare(method, Constants.Http.Methods.Head, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static bool IsBodyRequested(string method)
        {
            return string.Compare(method, Constants.Http.Methods.Get, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion
    }
}
