using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Wrappers;

namespace Ormikon.Owin.Static.Mapping
{
    internal class MapMiddleware : OwinMiddleware
    {
        private readonly string path;
        private readonly Func<IDictionary<string, object>, Task> mappedMiddleware;

        public MapMiddleware(Func<IDictionary<string, object>, Task> next,
            string path, Func<IDictionary<string, object>, Task> mappedMiddleware)
            : base(next)
        {
            path = path ?? "";
            if (path.EndsWith("/", StringComparison.Ordinal))
                path = path.TrimEnd('/');
            this.path = path;
            this.mappedMiddleware = mappedMiddleware;
        }

        protected override Task Invoke(IOwinContext context)
        {
            var location = context.Request.Location;
            if (PathMatches(location.Path))
            {
                context.Request.PathBase = location.PathBase + path;
                context.Request.Path = location.Path.Substring(path.Length);
                mappedMiddleware(context.Environment)
                    .ContinueWith(
                        task =>
                            {
                                context.Request.Path = location.Path;
                                context.Request.PathBase = location.PathBase;
                                task.Wait();
                            }, TaskContinuationOptions.ExecuteSynchronously);
            }

            return Next(context);
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
