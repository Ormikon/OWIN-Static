using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static
{
    internal class StaticMiddleware : StaticMiddlewareBase
    {
        private static readonly char[] indexFileSeparator = new[] { ';' };

        private readonly string[] sources;
        private readonly string[] indexFiles;
        private readonly bool redirectIfFolder;
        private readonly FileFilter include;
        private readonly FileFilter exclude;
        private readonly FileFilter unixHidden;
        private readonly bool allowHidden;
        private readonly DateTimeOffset expires;
        private readonly int maxAge;

        public StaticMiddleware(Func<IDictionary<string, object>, Task> next, StaticSettings settings)
            : base(next, settings.Cached, settings.Cache, settings.Expires, settings.MaxAge)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            sources = settings.Sources;
            if (sources == null || sources.Length == 0)
                throw new ArgumentException("Sources count should be one or more.", "settings");
            sources = NormalizeSources(sources);
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
                result.Expires = expires;
            if (maxAge != 0)
                result.MaxAge = maxAge;
            var fileInfo = info as FileInfo;
            if (fileInfo != null)
            {
                result.LastModified = fileInfo.LastWriteTimeUtc;
                result.ContentLength = fileInfo.Length;
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
            foreach (string fullPath in sources.Select(src => isPathEmpty ? src : Path.Combine(src, pathStr)))
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
