using Ormikon.Owin.Static;
using Ormikon.Owin.Static.Extensions;

// ReSharper disable CheckNamespace

namespace Owin
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Extension methods for OWIN to add the StaticMiddleware to the pipeline
    /// </summary>
    public static class StaticExtensions
    {
        /// <summary>
        /// Adds the StaticMiddleware to the pipeline
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <param name="settings">Static settings</param>
        /// <returns>App builder</returns>
        public static IAppBuilder UseStatic(this IAppBuilder appBuilder, StaticSettings settings)
        {
            return appBuilder.Use(typeof (StaticMiddleware), settings);
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <param name="sources">A collection of the source paths</param>
        /// <returns>App builder</returns>
// ReSharper disable MethodOverloadWithOptionalParameter
        public static IAppBuilder UseStatic(this IAppBuilder appBuilder, params string[] sources)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            return appBuilder.Use(typeof (StaticMiddleware), new StaticSettings(sources));
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <param name="sources">Single source path or a collection separated by comma</param>
        /// <returns>App builder</returns>
        public static IAppBuilder UseStatic(this IAppBuilder appBuilder, string sources)
        {
            return appBuilder.UseStatic(new StaticSettings(sources));
        }

        /// <summary>
        /// Maps the StaticMiddleware to the pipeline with custom path
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="settings">Static settings</param>
        /// <returns>App builder</returns>
        public static IAppBuilder MapStatic(this IAppBuilder appBuilder, string pathMatch, StaticSettings settings)
        {
            return appBuilder.Map(pathMatch, b => b.UseStatic(settings));
        }

        /// <summary>
        /// Maps the StaticMiddleware to the pipeline with custom path
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="sources">A collection of the source paths</param>
        /// <returns>App builder</returns>
// ReSharper disable MethodOverloadWithOptionalParameter
        public static IAppBuilder MapStatic(this IAppBuilder appBuilder, string pathMatch, params string[] sources)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            return appBuilder.Map(pathMatch, b => b.UseStatic(sources));
        }

        /// <summary>
        /// Maps the StaticMiddleware to the pipeline with custom path
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="sources">Single source path or a collection separated by comma</param>
        /// <returns>App builder</returns>
        public static IAppBuilder MapStatic(this IAppBuilder appBuilder, string pathMatch, string sources)
        {
            return appBuilder.Map(pathMatch, b => b.UseStatic(sources));
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline with default configuration
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <returns>App builder</returns>
        public static IAppBuilder UseStatic(this IAppBuilder appBuilder)
        {
            return appBuilder.UseStatic(".");
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline with default configuration.
        /// </summary>
        /// <param name="appBuilder">App builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <returns>App builder</returns>
        public static IAppBuilder MapStatic(this IAppBuilder appBuilder, string pathMatch)
        {
            return appBuilder.Map(pathMatch, app => app.UseStatic());
        }
    }
}