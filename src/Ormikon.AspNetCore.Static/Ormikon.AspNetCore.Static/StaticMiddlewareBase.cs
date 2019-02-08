using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ormikon.AspNetCore.Static.Cache;
using Ormikon.AspNetCore.Static.Filters;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.ResponseSender;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static
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

        protected override async Task InvokeAsync(IWrappedContext context, CancellationToken cancellationToken)
        {
            if (IsMethodAllowed(context.Request.Method) && await ProcessStaticIfFoundAsync(context, cancellationToken))
            {
                return;
            }
            await Next(context);
        }

        #region abstract methods

        protected abstract Task<StaticResponse> GetResponseAsync(Location location, CancellationToken cancellationToken);

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

        private async Task<CachedResponse> CacheGetAsync(Location location, CancellationToken cancellationToken)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            return await c.GetAsync(location.FullPath, cancellationToken);
        }

        private async Task CacheSetAsync(string path, CachedResponse data, CancellationToken cancellationToken)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            await c.SetAsync(path, data, GetCacheOffset(), cancellationToken);
        }

        private async Task ProcessResponseStreamAsync(IStaticResponse response, Stream stream, IWrappedContext ctx, CancellationToken cancellationToken)
        {
            var sender = responseSenderFactory.CreateSenderFor(response, ctx);
            await sender.SendAsync(response, stream, ctx, cancellationToken);
        }

        private async Task SendResponseAsync(CachedResponse cachedResponse, IWrappedContext ctx, CancellationToken cancellationToken)
        {
            await ProcessResponseStreamAsync(cachedResponse, cachedResponse.CreateBodyStream(), ctx, cancellationToken);
        }

        private async Task SendResponseAsync(StaticResponse staticResponse, IWrappedContext ctx, CancellationToken cancellationToken)
        {
            await ProcessResponseStreamAsync(staticResponse, staticResponse.Body, ctx, cancellationToken);
        }

        private async Task CacheResponseAndSendAsync(StaticResponse staticResponse, IWrappedContext ctx, CancellationToken cancellationToken)
        {
            if (cached)
            {
                var response = await CachedResponse.CreateAsync(staticResponse, cancellationToken);
                await CacheSetAsync(ctx.Request.Location.FullPath, response, cancellationToken);
                await SendResponseAsync(response, ctx, cancellationToken);
                return;
            }

            await SendResponseAsync(staticResponse, ctx, cancellationToken);
        }

        private async Task<bool> ProcessStaticIfFoundAsync(IWrappedContext ctx, CancellationToken cancellationToken)
        {
            var location = ctx.Request.Location;
            if (cached)
            {
                var response = await CacheGetAsync(location, cancellationToken);
                if (response != null)
                {
                    await SendResponseAsync(response, ctx, cancellationToken);
                    return true;
                }
            }

            var staticResponse = await GetResponseAsync(location, cancellationToken);

            if (staticResponse != null)
            {
                await CacheResponseAndSendAsync(staticResponse, ctx, cancellationToken);
                return true;
            }

            return false;
        }

        private static bool IsMethodAllowed(string method)
        {
            return string.Compare(method, Constants.Http.Methods.Get, StringComparison.OrdinalIgnoreCase) == 0
                   || string.Compare(method, Constants.Http.Methods.Head, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion
    }
}
