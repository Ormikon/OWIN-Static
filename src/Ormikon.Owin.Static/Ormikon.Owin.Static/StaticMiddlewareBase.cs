using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.Owin;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static
{
    internal abstract class StaticMiddlewareBase : OwinMiddleware
    {
        private static readonly char[] indexFileSeparator = new[] { ';' };

        private readonly string[] sources;
        private readonly bool cached;
        private readonly ObjectCache cache;
        private readonly DateTimeOffset expires;
        private readonly int maxAge;
        private readonly string[] indexFiles;
        private readonly bool redirectIfFolder;
        private readonly FileFilter include;
        private readonly FileFilter exclude;
        private readonly bool allowHidden;

        protected StaticMiddlewareBase(OwinMiddleware next, StaticSettings settings) :
            base(next)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            sources = settings.Sources;
            if (sources == null || sources.Length == 0)
                throw new ArgumentException("Sources count should be one or more.", "settings");
            sources = NormalizeSources(sources);
            cached = settings.Cached;
            cache = settings.Cache;
            expires = settings.Expires;
            maxAge = settings.MaxAge;
            indexFiles = ParseIndexFileString(settings.DefaultFile);
            redirectIfFolder = settings.RedirectIfFolderFound;
            include = new FileFilter(settings.Include);
            exclude = new FileFilter(settings.Exclude);
            allowHidden = settings.AllowHidden;
        }

        public override Task Invoke(IOwinContext context)
        {
            return IsMethodAllowed(context)
                       ? ProcessStaticIfFound(context) ?? Next.Invoke(context)
                       : Next.Invoke(context);
        }

        #region abstract methods

        protected abstract string ResolveResource(PathString path, out bool isFolder);

        protected abstract Stream GetResourceStream(string path);

        #endregion

        #region private methods

        private static string[] NormalizeSources(IEnumerable<string> sources)
        {
            return sources.Select(s => s.NormalizePath().GetFullPathForLocalPath()).ToArray();
        }

        private static string[] ParseIndexFileString(string indexFile)
        {
            if (string.IsNullOrWhiteSpace(indexFile))
                return null;
            string[] files = indexFile.Split(indexFileSeparator, StringSplitOptions.RemoveEmptyEntries)
                                      .Where(f => !string.IsNullOrWhiteSpace(f))
                                      .ToArray();
            return files.Length == 0 ? null : files;
        }

        private static Task SendStreamAsync(Stream from, Stream to)
        {
            return from.CopyToAsync(to)
                .ContinueWith(
                task =>
                {
                    from.Close();
                    if (task.Exception != null)
                        throw new AggregateException(task.Exception);
                });
        }

        private static Task RedirectToFolder(IOwinContext ctx)
        {
            string newLocation = ctx.Request.PathBase.HasValue
                                     ? ctx.Request.PathBase.Value + ctx.Request.Path.Value + "/"
                                     : ctx.Request.Path.Value + "/";
            ctx.Response.Redirect(newLocation);
            return ctx.Response.WriteAsync("Redirecting to " + newLocation);
        }

        private DateTimeOffset GetCacheOffset()
        {
            if (maxAge > 0)
                return DateTimeOffset.Now.AddSeconds(maxAge);
            if (expires != DateTimeOffset.MinValue && expires > DateTimeOffset.Now)
                return expires;
            return DateTimeOffset.MaxValue;// never expires
        }

        private byte[] CacheGet(string path)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            return c.Get(path) as byte[];
        }

        private void CacheSet(string path, byte[] data)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            c.Set(path, data, GetCacheOffset());
        }

        private Task SendResourceAsync(string path, string requestPath, Stream responseStream)
        {
            Stream s;
            if (cached)
            {
                requestPath = requestPath ?? "";
                byte[] cachedData;
                if ((cachedData = CacheGet(requestPath)) != null)
                {
                    s = new MemoryStream(cachedData);
                }
                else
                {
                    s = GetResourceStream(path);
                    var ms = new MemoryStream();
                    return SendStreamAsync(s, ms)
                        .ContinueWith(
                        task =>
                        {
                            if (task.Exception == null)
                            {
                                CacheSet(path, ms.ToArray());
                            }
                            else
                            {
                                ms.Close();
                                throw new AggregateException(task.Exception);
                            }
                            ms.Seek(0, SeekOrigin.Begin);
                            ms.CopyTo(responseStream);
                            ms.Close();
                        });
                }
            }
            else
            {
                s = GetResourceStream(path);
            }
            return SendStreamAsync(s, responseStream);
        }

        private static void AddMaxAgeHeader(int maxAge, IOwinResponse response)
        {
            if (maxAge > 0)
                response.Headers["Cache-Control"] = "public, max-age=" + maxAge;
        }

        private Task ProcessStaticIfFound(IOwinContext ctx)
        {
            bool isFolder;
            string resourcePath = ResolveResource(ctx.Request.Path, out isFolder);
            
            if (string.IsNullOrEmpty(resourcePath))
                return null;

            if ((include.IsActive() && !include.Contains(resourcePath)) ||
                (exclude.IsActive() && exclude.Contains(resourcePath)))
                return null;

            if (isFolder)
            {
                return redirectIfFolder ? RedirectToFolder(ctx) : null;
            }

            ctx.Response.ContentType = resourcePath.GetContentType();
            if (expires > DateTimeOffset.MinValue)
                ctx.Response.Expires = expires;
            AddMaxAgeHeader(maxAge, ctx.Response);

            return IsBodyRequested(ctx)
                       ? SendResourceAsync(resourcePath, ctx.Request.Path.Value, ctx.Response.Body)
                       : Task.FromResult((object) null);
        }

        private static bool IsMethodAllowed(IOwinContext ctx)
        {
            string method = ctx.Request.Method.ToUpperInvariant();
            return method == "GET" || method == "HEAD";
        }

        private static bool IsBodyRequested(IOwinContext ctx)
        {
            return ctx.Request.Method.ToUpperInvariant() == "GET";
        }

        #endregion

        #region config properties

        protected string[] Sources
        {
            get { return sources; }
        }

        protected string[] IndexFiles
        {
            get { return indexFiles; }
        }

        protected bool AllowHidden
        {
            get { return allowHidden; }
        }

        #endregion
    }
}
