using Microsoft.Owin;
using Ormikon.Owin.Static.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static
{
    internal class StaticMiddleware : OwinMiddleware
    {
        private readonly string[] sources;
        private readonly IDictionary<string, byte[]> cache;
        private readonly DateTimeOffset expires;
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
            cache = settings.Cached ? new ConcurrentDictionary<string, byte[]>() : null;
            expires = settings.Expires;
            include = new FileFilter(settings.Include);
            exclude = new FileFilter(settings.Exclude);
        }

        public override Task Invoke(IOwinContext context)
        {
            return ProcessStaticIfFound(context) ?? Next.Invoke(context);
        }

        #region private methods

        private static string[] NormalizeSources(string[] sources)
        {
            return sources.Select(s => s.NormalizePath().GetFullPathForLocalPath()).ToArray();
        }

        private static Stream OpenFileStream(string fileName)
        {
            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private Task SendStreamAsync(Stream from, Stream to)
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

        private Task SendFileAsync(string fileName, IOwinContext ctx)
        {
            Stream s;
            if (cache != null)
            {
                string path = ctx.Request.Path.Value.NormalizePath() ?? "";
                byte[] cachedData;
                if (cache.TryGetValue(path, out cachedData))
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
                                cache[path] = ms.GetBuffer();
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

        private Task ProcessStaticIfFound(IOwinContext ctx)
        {
            string fileName = ctx.Request.Path.GetLocalFileName(sources);
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            if ((include.IsActive() && !include.Contains(fileName)) ||
                (exclude.IsActive() && exclude.Contains(fileName)))
                return null;

            ctx.Response.ContentType = fileName.GetContentType();
            if (expires > DateTimeOffset.MinValue)
                ctx.Response.Expires = expires;
            return SendFileAsync(fileName, ctx);
        }

        #endregion
    }
}
