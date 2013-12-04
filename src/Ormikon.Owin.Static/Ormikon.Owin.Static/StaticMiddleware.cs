using System;
using System.Linq;
using Microsoft.Owin;
using Ormikon.Owin.Static.Extensions;
using System.IO;

namespace Ormikon.Owin.Static
{
    internal class StaticMiddleware : StaticMiddlewareBase
    {
        public StaticMiddleware(OwinMiddleware next, StaticSettings settings) : base(next, settings)
        {
        }

        protected override string ResolveResource(PathString path, out bool isFolder)
        {
            return GetLocalFileName(path, Sources, IndexFiles, AllowHidden, out isFolder);
        }

        protected override Stream GetResourceStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        #region private methods

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

        public static string GetLocalFileName(PathString path, string[] sources, string[] indexFiles,
                                              bool allowHidden, out bool isFolder)
        {
            isFolder = false;
            bool isFolderRequested = path.Value.EndsWith("/", StringComparison.Ordinal);
            string pathStr = path.Value.NormalizePath();

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
