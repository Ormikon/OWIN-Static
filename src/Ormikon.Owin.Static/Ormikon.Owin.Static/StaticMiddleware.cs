using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Extensions;
using System.IO;

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
            allowHidden = settings.AllowHidden;
            expires = settings.Expires;
            maxAge = settings.MaxAge;
        }

        protected override StaticResponse GetResponse(Location location)
        {
            bool isFolder;
            string localFileName = GetLocalFileName(location.Path, sources, indexFiles, allowHidden, out isFolder);
            if (string.IsNullOrEmpty(localFileName))
                return null;

            if ((include.IsActive() && !include.Contains(localFileName)) ||
                (exclude.IsActive() && exclude.Contains(localFileName)))
                return null;

            if (isFolder)
                return redirectIfFolder ? new StaticResponse(GetFolderRedirectLocation(location)) : null;

            return new StaticResponse(localFileName.GetContentType(), expires, maxAge,
                                      GetFileStream(localFileName));
        }

        #region private methods

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

        private static bool TryFindFile(string fileName, bool allowHidden)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;
            try
            {
                if (File.Exists(fileName))
                {
                    if (allowHidden)
                        return true;
                    return (File.GetAttributes(fileName) & FileAttributes.Hidden) != FileAttributes.Hidden &&
                           !fileName.IsUnixHidden();
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryFindFolder(string folderName, bool allowHidden)
        {
            if (string.IsNullOrEmpty(folderName))
                return false;
            try
            {
                if (Directory.Exists(folderName))
                {
                    if (allowHidden)
                        return true;
                    return (new DirectoryInfo(folderName).Attributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
                           !folderName.IsUnixHidden();
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryResolvePath(string path, bool lookForFolders, bool allowHidden, out bool isFolder)
        {
            isFolder = false;
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }
            if (TryFindFile(path, allowHidden))
                return true;
            if (lookForFolders && TryFindFolder(path, allowHidden))
            {
                isFolder = true;
                return true;
            }
            return false;
        }

        public static string GetLocalFileName(string path, string[] sources, string[] indexFiles,
                                              bool allowHidden, out bool isFolder)
        {
            isFolder = false;
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
                        if (TryResolvePath(fullPath, false, allowHidden, out isFolder))
                            return fullPath;
                    }
                }

                return null;
            }
            if (string.IsNullOrEmpty(pathStr))
            {
                if (sources.Length == 1 && TryResolvePath(sources[0], true, allowHidden, out isFolder))
                {
                    return sources[0];
                }
                return null;
            }
            foreach (string fullPath in sources.Select(source => Path.Combine(source, pathStr)))
            {
                if (TryResolvePath(fullPath, true, allowHidden, out isFolder))
                    return fullPath;
            }
            return null;
        }

        #endregion
    }
}
