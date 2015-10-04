using System.Linq;
using Ormikon.Owin.Static;
using Ormikon.Owin.Static.Config;
using Ormikon.Owin.Static.Extensions;
using System;

// ReSharper disable CheckNamespace

namespace Owin
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Extension methods for OWIN to add the StaticMiddleware to the pipeline
    /// </summary>
    public static class StaticExtensions
    {
        private static string CombinePaths(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
                return path2;
            if (string.IsNullOrEmpty(path2))
                return path1;
            if (path1.EndsWith("/", StringComparison.Ordinal))
            {
                return path2.StartsWith("/", StringComparison.Ordinal)
                    ? path1 + path2.Substring(1)
                    : path1 + path2;
            }
            return !path2.StartsWith("/", StringComparison.Ordinal)
                ? path1 + "/" + path2
                : path1 + path2;
        }

        private static IAppBuilder UseConfigValues(this IAppBuilder appBuilder, string mapPath, Section section)
        {
            return section.EnumerateSettings()
                .Aggregate(appBuilder,
                    (current, map) =>
                        map.HasPath || !string.IsNullOrEmpty(mapPath)
                            ? current.MapStatic(CombinePaths(mapPath, map.HasPath ? map.Path : null), map.Value)
                            : current.UseStatic(map.Value));
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline with configuration from the configuration file (app.config)
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <returns>App builder</returns>
        public static IAppBuilder UseStaticConfig(this IAppBuilder appBuilder)
        {
            var cs = Section.Default;
            if (cs == null)
                return appBuilder.UseStatic();

            return appBuilder.UseConfigValues(cs.MapPath, cs);
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline with configuration from the configuration file (app.config)
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <returns>App builder</returns>
        public static IAppBuilder MapStaticConfig(this IAppBuilder appBuilder, string pathMatch)
        {
            var cs = Section.Default;
            if (cs == null)
                return appBuilder.MapStatic(pathMatch);
            return appBuilder.UseConfigValues(CombinePaths(pathMatch, cs.MapPath), cs);
        }
    }
}