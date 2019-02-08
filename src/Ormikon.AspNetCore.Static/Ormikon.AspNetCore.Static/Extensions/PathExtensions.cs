using System;
using System.IO;

namespace Ormikon.AspNetCore.Static.Extensions
{
    internal static class PathExtensions
    {
        public static string NormalizePath(this string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            return path.Trim('/', '\\', ' ', '\t')
                .Replace('\\', Path.DirectorySeparatorChar) // windows -> linux
                .Replace('/', Path.DirectorySeparatorChar); // linux -> windows
        }

        private static string NormalizePathEnd(this string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            return path.TrimEnd('/', '\\');
        }

        public static string GetFullPathForLocalPath(this string localPath, string hostDir = null)
        {
            if (string.IsNullOrEmpty(localPath))
                return (string.IsNullOrEmpty(hostDir) ? Directory.GetCurrentDirectory() : hostDir).NormalizePathEnd();

            if (string.IsNullOrEmpty(hostDir) || Path.IsPathRooted(localPath))
            {
                return Path.GetFullPath(localPath).NormalizePathEnd();
            }

            return Path.GetFullPath(Path.Combine(hostDir, localPath)).NormalizePathEnd();
        }

        public static bool IsUnixHidden(this string name)
        {
            return name.IndexOf(Path.DirectorySeparatorChar + ".", StringComparison.Ordinal) >= 0;
        }
    }
}
