using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ormikon.AspNetCore.Static.Filters;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;
using Ormikon.AspNetCore.Static.Extensions;

// Make internals visible for the test assembly
[assembly: InternalsVisibleTo("Ormikon.AspNetCore.Static.Tests")]

namespace Ormikon.AspNetCore.Static
{
    internal abstract class StaticFilesMiddlewareBase : StaticMiddlewareBase
    {
        protected enum EntityType
        {
            Directory,
            File
        }

        protected class EntityInfo
        {
            public EntityInfo(string fullName)
                : this(EntityType.Directory, fullName, DateTime.MinValue, 0)
            {
            }

            public EntityInfo(string fullName, DateTime lastWriteTimeUtc, long length)
                : this(EntityType.File, fullName, lastWriteTimeUtc, length)
            {
            }

            private EntityInfo(EntityType type, string fullName, DateTime lastWriteTimeUtc, long length)
            {
                Type = type;
                FullName = fullName;
                LastWriteTimeUtc = lastWriteTimeUtc;
                Length = length;
            }

            public EntityType Type { get; }

            public string FullName { get; }

            public DateTime LastWriteTimeUtc { get; }

            public long Length { get; }
        }

        private static readonly char[] indexFileSeparator = { ';' };

        private readonly string[] indexFiles;
        private readonly bool redirectIfFolder;
        private readonly IFilter include;
        private readonly IFilter exclude;
        private readonly DateTimeOffset expires;
        private readonly int maxAge;

        protected StaticFilesMiddlewareBase(RequestDelegate next, StaticSettingsBase settings)
            : base(next, settings.Cached, settings.Cache, settings.Expires, settings.MaxAge,
                settings.CompressedContentTypes)
        {
            indexFiles = ParseIndexFileString(settings.DefaultFile);
            redirectIfFolder = settings.RedirectIfFolderFound;
            include = new FileFilter(settings.Include);
            exclude = new FileFilter(settings.Exclude);
            expires = settings.Expires;
            maxAge = settings.MaxAge;
        }

        protected sealed override Task<StaticResponse> GetResponseAsync(Location location, CancellationToken cancellationToken)
        {
            if (!TestFilters(location))
                return Task.FromResult<StaticResponse>(null);

            var info = GetLocalFileInfo(location);
            if (info == null)
                return Task.FromResult<StaticResponse>(null);

            if (info.Type == EntityType.Directory)
                return Task.FromResult(redirectIfFolder ? StaticResponse.Redirect(GetFolderRedirectLocation(location)) : null);

            var result = new StaticResponse(info.FullName.GetContentType(), GetFileStream(info.FullName));
            if (expires != DateTimeOffset.MinValue)
                result.Headers.Expires.Value = expires;
            if (maxAge != 0)
                result.Headers.CacheControl.MaxAge = maxAge;
            if (info.Type == EntityType.File)
            {
                result.Headers.LastModified.Value = info.LastWriteTimeUtc;
                result.Headers.ContentLength.Value = info.Length;
            }

            return Task.FromResult(result);
        }

        #region protected virtual methods

        protected virtual bool TestFilters(Location location)
        {
            return !((include.IsActive() && !include.Contains(location.Path))
                || (exclude.IsActive() && exclude.Contains(location.Path)));
        }

        protected abstract Stream GetFileStream(string path);

        protected abstract EntityInfo GetLocalFileInfo(string path);

        #endregion

        #region private methods

        private EntityInfo GetLocalFileInfo(Location location)
        {
            var isFolderRequested = GetIsFolderRequested(location);
            if (isFolderRequested)
            {
                if (indexFiles == null || indexFiles.Length == 0)
                    return null;

                string path = location.Path.Value.NormalizePath();

                foreach (var indexFile in indexFiles)
                {
                    string fullPath = string.IsNullOrEmpty(path)
                        ? indexFile
                        : Path.Combine(path, indexFile);
                    var info = GetLocalFileInfo(fullPath);
                    if (info != null)
                    {
                        return info;
                    }
                }

                return null;
            }

            return GetLocalFileInfo(location.Path.Value.NormalizePath());
        }

        private static bool GetIsFolderRequested(Location location)
        {
            return location.Path.Value.EndsWith("/", StringComparison.Ordinal);
        }

        private static string GetFolderRedirectLocation(Location location)
        {
            return location.FullPath + "/";
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

        #endregion
    }
}
