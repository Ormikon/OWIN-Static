using System.Linq;
using Microsoft.Owin;
using System;
using System.IO;

namespace Ormikon.Owin.Static.Extensions
{
    internal static class PathExtensions
    {
        public static string NormalizePath(this string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            return path.Trim('/', '\\', ' ', '\t')
                .Replace('\\', Path.DirectorySeparatorChar) // windows -> linux
                .Replace('/', Path.DirectorySeparatorChar); // linux -> windows
        }

        public static string GetFullPathForLocalPath(this string localPath)
        {
            if (string.IsNullOrEmpty(localPath))
                return Directory.GetCurrentDirectory().NormalizePath();
            return Path.GetFullPath(localPath).NormalizePath();
        }

        private static bool TryFindFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;
            try
            {
                return File.Exists(fileName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryFindFolder(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                return false;
            try
            {
                return Directory.Exists(folderName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetLocalFileName(this PathString path, string[] sources, string indexFile, out bool isFolder)
        {
            isFolder = false;
            bool isFolderRequested = path.Value.EndsWith("/", StringComparison.Ordinal);
            string pathStr = path.Value.NormalizePath();
            if (isFolderRequested)
            {
                if (string.IsNullOrEmpty(indexFile))
                    return null;
                pathStr = string.IsNullOrEmpty(pathStr) ? indexFile : Path.Combine(pathStr, indexFile);
            }
            if (string.IsNullOrEmpty(pathStr))
            {
                return sources.Length != 1 ? null : TryFindFile(sources[0]) ? sources[0] : null;
            }
            if (pathStr.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return null;
            }
            string result = sources.Select(t => Path.Combine(t, pathStr)).FirstOrDefault(TryFindFile);
            if (result == null && !isFolderRequested)
            {
                result = sources.Select(t => Path.Combine(t, pathStr)).FirstOrDefault(TryFindFolder);
                isFolder = result != null;
            }
            return result;
        }
    }
}
