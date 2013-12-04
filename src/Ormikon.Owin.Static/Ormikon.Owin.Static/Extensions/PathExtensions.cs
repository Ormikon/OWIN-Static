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

        public static bool IsUnixHidden(this string name)
        {
            return name.IndexOf(Path.DirectorySeparatorChar + ".", StringComparison.Ordinal) >= 0;
        }
    }
}
