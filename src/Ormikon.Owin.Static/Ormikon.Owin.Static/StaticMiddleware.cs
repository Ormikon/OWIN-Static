using System.Runtime.Caching;
using Microsoft.Owin;
using Ormikon.Owin.Static.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static
{
    internal class StaticMiddleware : OwinMiddleware
    {
        private readonly string[] sources;
        private readonly bool cached;
        private readonly ObjectCache cache;
        private readonly DateTimeOffset expires;
        private readonly int maxAge;
        private readonly string indexFile;
        private readonly bool redirectIfFolder;
        private readonly FileFilter include;
        private readonly FileFilter exclude;

        public StaticMiddleware(OwinMiddleware next, StaticSettings settings) :
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
            indexFile = settings.DefaultFile;
            redirectIfFolder = settings.RedirectIfFolderFound;
            include = new FileFilter(settings.Include);
            exclude = new FileFilter(settings.Exclude);
        }

        public override Task Invoke(IOwinContext context)
        {
            return ProcessStaticIfFound(context) ?? Next.Invoke(context);
        }

        #region private methods

        private static string[] NormalizeSources(IEnumerable<string> sources)
        {
            return sources.Select(s => s.NormalizePath().GetFullPathForLocalPath()).ToArray();
        }

        private static Stream OpenFileStream(string fileName)
        {
            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

        private Task SendFileAsync(string fileName, IOwinContext ctx)
        {
            Stream s;
            if (cached)
            {
                string path = ctx.Request.Path.Value.NormalizePath() ?? "";
                byte[] cachedData;
                if ((cachedData = CacheGet(path)) != null)
                {
                    s = new MemoryStream(cachedData);
                }
                else
                {
                    s = OpenFileStream(fileName);
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
                            ms.CopyTo(ctx.Response.Body);
                            ms.Close();
                        });
                }
            }
            else
            {
                s = OpenFileStream(fileName);
            }
            return SendStreamAsync(s, ctx.Response.Body);
        }

        private static void AddMaxAgeHeader(int maxAge, IOwinResponse response)
        {
            if (maxAge > 0)
                response.Headers["Cache-Control"] = "public, max-age=" + maxAge;
        }

        private Task ProcessStaticIfFound(IOwinContext ctx)
        {
            bool isFolder;
            string fileName = ctx.Request.Path.GetLocalFileName(sources, indexFile, out isFolder);
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            if ((include.IsActive() && !include.Contains(fileName)) ||
                (exclude.IsActive() && exclude.Contains(fileName)))
                return null;

            if (isFolder)
            {
                return redirectIfFolder ? RedirectToFolder(ctx) : null;
            }

            ctx.Response.ContentType = fileName.GetContentType();
            if (expires > DateTimeOffset.MinValue)
                ctx.Response.Expires = expires;
            AddMaxAgeHeader(maxAge, ctx.Response);
            return SendFileAsync(fileName, ctx);
        }

        #endregion
    }
}
