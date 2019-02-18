using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Ormikon.AspNetCore.Static.Filters;
using Ormikon.AspNetCore.Static.Wrappers;
using Ormikon.AspNetCore.Static.Extensions;

namespace Ormikon.AspNetCore.Static
{
    internal class StaticMiddleware : StaticFilesMiddlewareBase
    {
        private readonly string[] sources;
        private readonly IFilter unixHidden;
        private readonly bool allowHidden;

        public StaticMiddleware(RequestDelegate next, StaticSettings settings, IHostingEnvironment hostEnvironment)
            : base(next, settings)
        {
            sources = settings.Sources ?? new[] {"\\"};
            if (sources.Length == 0)
                throw new ArgumentException("Sources count should be one or more.", nameof(settings));
            sources = NormalizeSources(sources, hostEnvironment.WebRootPath);
            unixHidden = new FileFilter(allowHidden ? null : @"**\.*");
            allowHidden = settings.AllowHidden;
        }

        #region protected overridden methods

        protected sealed override bool TestFilters(Location location)
        {
            return !(unixHidden.IsActive() && unixHidden.Contains(location.Path))
                   && base.TestFilters(location);
        }

        protected override Stream GetFileStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        protected override EntityInfo GetLocalFileInfo(string path)
        {
            bool isPathEmpty = string.IsNullOrEmpty(path);
            // ReSharper disable AssignNullToNotNullAttribute
            foreach (string fullPath in sources.Select(src => isPathEmpty ? src : Path.Combine(src, path)))
            // ReSharper restore AssignNullToNotNullAttribute
            {
                var info = TryResolvePath(fullPath, true, allowHidden);
                if (info != null)
                    return AsEntityInfo(info);
            }

            return null;
        }

        #endregion

        #region private methods

        private static string[] NormalizeSources(IEnumerable<string> sources, string webRoot)
        {
            return sources.Select(s => (s ?? ".").NormalizePath().GetFullPathForLocalPath(webRoot)).ToArray();
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

        private static EntityInfo AsEntityInfo(FileSystemInfo systemInfo)
        {
            if (systemInfo is null)
            {
                return null;
            }

            if (systemInfo is DirectoryInfo)
            {
                return new EntityInfo(systemInfo.FullName);
            }

            return new EntityInfo(systemInfo.FullName, systemInfo.LastWriteTimeUtc, ((FileInfo) systemInfo).Length);
        }

        #endregion
    }
}
