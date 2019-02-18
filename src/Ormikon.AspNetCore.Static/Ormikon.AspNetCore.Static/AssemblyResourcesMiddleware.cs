using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Ormikon.AspNetCore.Static
{
    internal sealed class AssemblyResourcesMiddleware : StaticFilesMiddlewareBase
    {
        private static readonly Regex startsWithDecimal = new Regex("^(\\d)", RegexOptions.Compiled);
        private static readonly Regex badPathChars = new Regex("[^\\w]", RegexOptions.Compiled);

        private readonly Assembly assembly;
        private readonly DateTime assemblyWriteTimeUtc;
        private readonly Dictionary<string, (string name, long size)> resourceFiles;
        private readonly HashSet<string> resourceFolders;

        public AssemblyResourcesMiddleware(RequestDelegate next, StaticResourcesSettings settings)
            : base(next, settings)
        {
            assembly = settings.Assembly ?? Assembly.GetEntryAssembly();
            assemblyWriteTimeUtc = File.GetLastWriteTimeUtc(assembly.ManifestModule.FullyQualifiedName);

            resourceFiles = BuildAssemblyFilePaths(assembly, settings.Resources);
            resourceFolders = CollectAssemblyFolderPaths(resourceFiles);
        }

        protected override Stream GetFileStream(string path)
        {
            string resourcePath = resourceFiles[path].name;
            return assembly.GetManifestResourceStream(resourcePath);
        }

        protected override EntityInfo GetLocalFileInfo(string path)
        {
            (string name, long size) nameAndSize;
            if (string.IsNullOrEmpty(path))
            {
                if (resourceFiles.Count == 1)
                {
                    nameAndSize = resourceFiles.Values.First();
                    return new EntityInfo(path, assemblyWriteTimeUtc, nameAndSize.size);
                }

                return new EntityInfo("");
            }

            var sanitizedName = SanitizeName(path);
            if (resourceFiles.TryGetValue(sanitizedName, out nameAndSize))
            {
                return new EntityInfo(sanitizedName, assemblyWriteTimeUtc, nameAndSize.size);
            }

            var sanitizedPath = SanitizePath(path);
            if (resourceFolders.Contains(sanitizedPath))
            {
                return new EntityInfo(sanitizedPath);
            }

            return null;
        }

        private static Dictionary<string, (string name, long size)> BuildAssemblyFilePaths(Assembly assembly, string resource)
        {
            var resourceNames = assembly
                .GetManifestResourceNames()
                .Where(rn =>
                    string.IsNullOrEmpty(resource) ||
                    rn.StartsWith(resource, StringComparison.OrdinalIgnoreCase));

            var result = new Dictionary<string, (string name, long size)>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var name in resourceNames)
            {
                NameToPaths(name, GetResourceSize(assembly, name), resource, result);
            }

            return result;
        }

        private static long GetResourceSize(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                return stream.Length;
            }
        }

        private static HashSet<string> CollectAssemblyFolderPaths(IDictionary<string, (string, long)> filePaths)
        {
            var paths = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var filePath in filePaths.Keys)
            {
                var pathIdx = filePath.LastIndexOf(Path.DirectorySeparatorChar);
                if (pathIdx > 0)
                {
                    var path = filePath.Remove(pathIdx);
                    if (!paths.Contains(path))
                    {
                        paths.Add(path);
                    }
                }
            }

            return paths;
        }

        private static void NameToPaths(string name, long size, string resource, IDictionary<string, (string name, long size)> paths)
        {
            var resPath = name;
            if (!string.IsNullOrEmpty(resource) && name.Length != resource.Length)
            {
                name = name.Substring(resource.Length);
                if (name[0] == '.')
                {
                    name = name.Substring(1);
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            name = name.ToLowerInvariant();

            int dotIdx = -1;
            do
            {
                if (dotIdx > 0)
                {
                    name = name.Remove(dotIdx) + Path.DirectorySeparatorChar + name.Substring(dotIdx + 1);
                }

                var sanitized = SanitizeName(name);

                if (!paths.ContainsKey(sanitized))
                {
                    paths.Add(sanitized, (resPath, size));
                }

                dotIdx = name.IndexOf('.');
            }
            while (dotIdx > 0);
        }

        private static string SanitizeName(string name)
        {
            var path = Path.GetDirectoryName(name);
            if (string.IsNullOrEmpty(path))
            {
                return name;
            }

            return Path.Combine(SanitizePath(path), Path.GetFileName(name));
        }

        private static string SanitizePath(string path)
        {
            return string.Join(Path.DirectorySeparatorChar,
                path.Split(Path.DirectorySeparatorChar)
                    .Select(dir =>
                        badPathChars.Replace(startsWithDecimal.Replace(path, "_$1"), "_")
                    ));
        }
    }
}
