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

        public static string GetLocalFileName(this PathString path, string[] sources)
        {
            string pathStr = path.Value.NormalizePath();
            if (string.IsNullOrEmpty(pathStr))
            {
                return sources.Length != 1 ? null : TryFindFile(sources[0]) ? sources[0] : null;
            }
            if (pathStr.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return null;
            }
            for (int i = 0; i < sources.Length; i++)
            {
                string fileNameInSource = Path.Combine(sources[i], pathStr);
                if (TryFindFile(fileNameInSource))
                    return fileNameInSource;
            }
            return null;
        }
    }
}
