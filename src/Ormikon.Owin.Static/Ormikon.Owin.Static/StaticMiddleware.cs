using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Ormikon.Owin.Static.Extensions;
using Ormikon.Owin.Static.Filters;
using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;

// Make internals visible for the test assembly
[assembly: InternalsVisibleTo("Ormikon.Owin.Static.Tests")]

namespace Ormikon.Owin.Static
{
    internal class StaticMiddleware : StaticMiddlewareBase
    {
        private static readonly char[] indexFileSeparator = { ';' };

        private readonly string[] sources;
        private readonly string[] indexFiles;
        private readonly bool redirectIfFolder;
        private readonly IFilter include;
        private readonly IFilter exclude;
        private readonly IFilter unixHidden;
        private readonly bool allowHidden;
        private readonly DateTimeOffset expires;
        private readonly int maxAge;

        public StaticMiddleware(RequestDelegate next, StaticSettings settings, IHostingEnvironment hostEnvironment)
            : base(next, settings.Cached, settings.Cache, settings.Expires, settings.MaxAge,
                settings.CompressedContentTypes)
        {
            sources = settings.Sources ?? new[] {"\\"};
            if (sources.Length == 0)
                throw new ArgumentException("Sources count should be one or more.", "settings");
            sources = NormalizeSources(sources, hostEnvironment.WebRootPath);
            indexFiles = ParseIndexFileString(settings.DefaultFile);
            redirectIfFolder = settings.RedirectIfFolderFound;
            include = new FileFilter(settings.Include);
            exclude = new FileFilter(settings.Exclude);
            unixHidden = new FileFilter(allowHidden ? null : @"**\.*");
            allowHidden = settings.AllowHidden;
            expires = settings.Expires;
            maxAge = settings.MaxAge;
        }

        protected override StaticResponse GetResponse(Location location)
        {
            if (!TestFilters(location))
                return null;

            var info = GetLocalFileInfo(location.Path, sources, indexFiles, allowHidden);
            if (info == null)
                return null;

            if (info is DirectoryInfo)
                return redirectIfFolder ? StaticResponse.Redirect(GetFolderRedirectLocation(location)) : null;

            var result = new StaticResponse(info.FullName.GetContentType(), GetFileStream(info.FullName));
            if (expires != DateTimeOffset.MinValue)
                result.Headers.Expires.Value = expires;
            if (maxAge != 0)
                result.Headers.CacheControl.MaxAge = maxAge;
            var fileInfo = info as FileInfo;
            if (fileInfo != null)
            {
                result.Headers.LastModified.Value = fileInfo.LastWriteTimeUtc;
                result.Headers.ContentLength.Value = fileInfo.Length;
            }

            return result;
        }

        #region private methods

        private bool TestFilters(Location location)
        {
            return !((unixHidden.IsActive() && unixHidden.Contains(location.Path))
                || (include.IsActive() && !include.Contains(location.Path))
                || (exclude.IsActive() && exclude.Contains(location.Path)));
        }

        private static Stream GetFileStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private static string GetFolderRedirectLocation(Location location)
        {
            return location.FullPath + "/";
        }

        private static string[] NormalizeSources(IEnumerable<string> sources, string webRoot)
        {
            return sources.Select(s => (s ?? ".").NormalizePath().GetFullPathForLocalPath(webRoot)).ToArray();
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

        private static FileSystemInfo GetFileInfo(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            try
            {
                return new FileInfo(fileName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static FileSystemInfo GetFolderInfo(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                return null;
            try
            {
                return new DirectoryInfo(folderName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool IsHidden(FileSystemInfo info)
        {
            return (info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        }

        private static FileSystemInfo TryResolvePath(string path, bool lookForFolders, bool allowHidden)
        {
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return null;
            }
            var info = GetFileInfo(path);
            if (info != null && info.Exists)
            {
                if (allowHidden)
                    return info;
                if (!IsHidden(info))
                    return info;
            }
            if (lookForFolders)
            {
                info = GetFolderInfo(path);
                if (info != null && info.Exists)
                {
                    if (allowHidden)
                        return info;
                    if (!IsHidden(info))
                        return info;
                }
            }
            return null;
        }

        public static FileSystemInfo GetLocalFileInfo(string path, string[] sources, string[] indexFiles,
                                                      bool allowHidden)
        {
            bool isFolderRequested = path.EndsWith("/", StringComparison.Ordinal);
            string pathStr = path.NormalizePath();

            if (isFolderRequested)
            {
                if (indexFiles == null || indexFiles.Length == 0)
                    return null;

                foreach (var source in sources)
                {
                    foreach (var indexFile in indexFiles)
                    {
                        string fullPath = string.IsNullOrEmpty(pathStr)
                                              ? Path.Combine(source, indexFile)
                                              : Path.Combine(source, pathStr, indexFile);
                        var info = TryResolvePath(fullPath, false, allowHidden);
                        if (info != null)
                            return info;
                    }
                }

                return null;
            }

            bool isPathEmpty = string.IsNullOrEmpty(pathStr);
// ReSharper disable AssignNullToNotNullAttribute
            foreach (string fullPath in sources.Select(src => isPathEmpty ? src : Path.Combine(src, pathStr)))
// ReSharper restore AssignNullToNotNullAttribute
            {
                var info = TryResolvePath(fullPath, true, allowHidden);
                if (info != null)
                    return info;
            }

            return null;
        }

        #endregion
    }
}
