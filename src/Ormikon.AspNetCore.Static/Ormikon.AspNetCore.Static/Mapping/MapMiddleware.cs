using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static.Mapping
{
    internal class MapMiddleware : BaseMiddleware
    {
        private readonly string path;
        private readonly RequestDelegate chain;

        public MapMiddleware(RequestDelegate next, string path, RequestDelegate chain)
            : base(next)
        {
            path = path ?? "";
            if (path.EndsWith("/", StringComparison.Ordinal))
                path = path.TrimEnd('/');
            this.path = path;
            this.chain = chain;
        }

        protected override async Task InvokeAsync(IWrappedContext context, CancellationToken cancellationToken)
        {
            var location = context.Request.Location;
            if (PathMatches(location.Path))
            {
                context.Request.PathBase = location.PathBase + path;
                context.Request.Path = location.Path.Value.Substring(path.Length);
                await chain(((WrappedContext) context).CoreContext);
                context.Request.Location = location;
                if (context.Response.StatusCode != 404)
                {
                    return;
                }
            }

            await Next(context);
        }

        private bool PathMatches(string requestPath)
        {
            requestPath = requestPath ?? "";
            if (requestPath.StartsWith(path, StringComparison.OrdinalIgnoreCase))
            {
                if (requestPath.Length == path.Length ||
                    (requestPath.Length > path.Length && requestPath[path.Length] == '/'))
                    return true;
            }
            return false;
        }
    }
}
