﻿using System;
using Ormikon.Owin.Static;
using Ormikon.Owin.Static.Mapping;

// ReSharper disable CheckNamespace

namespace Microsoft.AspNetCore.Builder
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
        /// <param name="builder">Application builder</param>
        /// <param name="settings">Static settings</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder UseStatic(this IApplicationBuilder builder, StaticSettings settings)
        {
            return builder.UseMiddleware<StaticMiddleware>(settings);
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="sources">A collection of the source paths</param>
        /// <returns>Application builder</returns>
// ReSharper disable MethodOverloadWithOptionalParameter
        public static IApplicationBuilder UseStatic(this IApplicationBuilder builder, params string[] sources)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            return builder.UseMiddleware<StaticMiddleware>(new StaticSettings(sources));
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="sources">Single source path or a collection separated by comma</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder UseStatic(this IApplicationBuilder builder, string sources)
        {
            return builder.UseStatic(new StaticSettings(sources));
        }

        /// <summary>
        /// Maps the StaticMiddleware to the pipeline with custom path
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="settings">Static settings</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder MapStatic(this IApplicationBuilder builder, string pathMatch, StaticSettings settings)
        {
            return Map(builder, pathMatch, b => b.UseStatic(settings));
        }

        /// <summary>
        /// Maps the StaticMiddleware to the pipeline with custom path
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="sources">A collection of the source paths</param>
        /// <returns>Application builder</returns>
// ReSharper disable MethodOverloadWithOptionalParameter
        public static IApplicationBuilder MapStatic(this IApplicationBuilder builder, string pathMatch, params string[] sources)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            return Map(builder, pathMatch, b => b.UseStatic(sources));
        }

        /// <summary>
        /// Maps the StaticMiddleware to the pipeline with custom path
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="sources">Single source path or a collection separated by comma</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder MapStatic(this IApplicationBuilder builder, string pathMatch, string sources)
        {
            return Map(builder, pathMatch, b => b.UseStatic(sources));
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline with default configuration
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder UseStatic(this IApplicationBuilder builder)
        {
            return builder.UseStatic(".");
        }

        /// <summary>
        /// Adds the StaticMiddleware to the pipeline with default configuration.
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder MapStatic(this IApplicationBuilder builder, string pathMatch)
        {
            return Map(builder, pathMatch, app => app.UseStatic());
        }

        /// <summary>
        /// Internal mapping middleware to continue request pipeline if file was not found.
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="path">Custom path</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Application builder</returns>
        private static IApplicationBuilder Map(this IApplicationBuilder builder, string path, Action<IApplicationBuilder> configuration)
        {
            var newBuilder = builder.New();
            configuration(newBuilder);
            var chain = newBuilder.Build();
            return builder.UseMiddleware<MapMiddleware>(path, chain);
        }
    }
}
