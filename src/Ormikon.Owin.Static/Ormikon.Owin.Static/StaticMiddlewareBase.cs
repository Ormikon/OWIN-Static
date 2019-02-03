using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ormikon.Owin.Static.ResponseSender;
using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;
using Ormikon.Owin.Static.Filters;
using Ormikon.Owin.Static.Cache;

namespace Ormikon.Owin.Static
{
    internal abstract class StaticMiddlewareBase : BaseMiddleware
    {
        private readonly bool cached;
        private readonly IStaticCache cache;
        private readonly IResponseSenderFactory responseSenderFactory;
        private readonly DateTimeOffset expires;
        private readonly int maxAge;

        protected StaticMiddlewareBase(RequestDelegate next)
            : this(next, false, null, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(RequestDelegate next, bool cached)
            : this(next, cached, null, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(RequestDelegate next, bool cached, IStaticCache cache)
            : this(next, cached, cache, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(RequestDelegate next, bool cached, IStaticCache cache,
            string compressedContentFilter)
            : this(next, cached, cache, DateTimeOffset.MinValue, 0, compressedContentFilter)
        {
        }

        protected StaticMiddlewareBase(RequestDelegate next, bool cached, IStaticCache cache,
            DateTimeOffset expires, int maxAge)
            : this(next, cached, cache, expires, maxAge, StaticSettings.DefaultCompressedTypesFilter)
        {
        }

        protected StaticMiddlewareBase(RequestDelegate next, bool cached, IStaticCache cache,
            DateTimeOffset expires, int maxAge, string compressedContentFilter)
            : this(next, cached, cache, expires, maxAge, new ContentTypeFilter(compressedContentFilter))
        {
        }

        protected StaticMiddlewareBase(RequestDelegate next, bool cached, IStaticCache cache,
            DateTimeOffset expires, int maxAge, IFilter compressedContentFilter)
            : this(next, cached, cache, expires, maxAge,
            new ResponseSenderFactory(compressedContentFilter))
        {
        }

        protected StaticMiddlewareBase(RequestDelegate next, bool cached, IStaticCache cache,
            DateTimeOffset expires, int maxAge, IResponseSenderFactory responseSenderFactory)
            : base(next)
        {
            this.cached = cached;
            this.cache = cache;
            this.expires = expires;
            this.maxAge = maxAge;
            this.responseSenderFactory = responseSenderFactory;
        }

        protected override Task Invoke(IWrappedContext context)
        {
            return IsMethodAllowed(context.Request.Method)
                    ? ProcessStaticIfFound(context) ?? Next(context)
                    : Next(context);
        }

        #region abstract methods

        protected abstract StaticResponse GetResponse(Location location);

        #endregion

        #region private methods

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
            return c.Get(location.FullPath);
        }

        private void CacheSet(string path, CachedResponse data)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            c.Set(path, data, GetCacheOffset());
        }

        private Task ProcessResponseStream(IStaticResponse response, Stream stream, IWrappedContext ctx)
        {
            var sender = responseSenderFactory.CreateSenderFor(response, ctx);
            return sender.SendAsync(response, stream, ctx);
        }

        private Task SendResponse(CachedResponse cachedResponse, IWrappedContext ctx)
        {
            return ProcessResponseStream(cachedResponse, cachedResponse.CreateBodyStream(), ctx);
        }

        private Task SendResponse(StaticResponse staticResponse, IWrappedContext ctx)
        {
            return ProcessResponseStream(staticResponse, staticResponse.Body, ctx);
        }

        private Task CacheResponseAndSend(StaticResponse staticResponse, IWrappedContext ctx)
        {
            if (cached)
            {
                return CachedResponse.CreateAsync(staticResponse, ctx.CallCancelled)
                        .ContinueWith(
                            task =>
                            {
                                task.Wait(ctx.CallCancelled);
                                CacheSet(ctx.Request.Location.FullPath, task.Result);
                                return SendResponse(task.Result, ctx);
                            }, TaskContinuationOptions.ExecuteSynchronously)
                        .Unwrap();
            }
            return SendResponse(staticResponse, ctx);
        }

        private Task ProcessStaticIfFound(IWrappedContext ctx)
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

        #endregion
    }
}
