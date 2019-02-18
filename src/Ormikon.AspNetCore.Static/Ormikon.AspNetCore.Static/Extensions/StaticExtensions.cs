using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ormikon.AspNetCore.Static;
using Ormikon.AspNetCore.Static.Mapping;
using Ormikon.AspNetCore.Static.Options;

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
            return builder.UseMiddleware<StaticOptionsMiddleware>();
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
        /// Use static files from resources of an entry Assembly
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder UseStaticResources(this IApplicationBuilder builder)
        {
            return builder.UseStaticResources((StaticResourcesSettings) null);
        }

        /// <summary>
        /// Use static files from resources of an entry Assembly
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder MapStaticResources(this IApplicationBuilder builder, string pathMatch)
        {
            return Map(builder, pathMatch, app => app.UseStaticResources());
        }

        /// <summary>
        /// Use static files from resources of <paramref name="assembly"/>
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="assembly">Assembly with resources</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder UseStaticResources(this IApplicationBuilder builder, Assembly assembly)
        {
            return builder.UseStaticResources(assembly, null);
        }

        /// <summary>
        /// Use static files from resources of <paramref name="assembly"/>
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="assembly">Assembly with resources</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder MapStaticResources(this IApplicationBuilder builder, string pathMatch, Assembly assembly)
        {
            return Map(builder, pathMatch, app => app.UseStaticResources(assembly));
        }

        /// <summary>
        /// Use static files from resources of <paramref name="assembly"/> and filtered by <paramref name="resources"/>
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="assembly">Assembly with resources</param>
        /// <param name="resources">Resources filter</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder UseStaticResources(this IApplicationBuilder builder, Assembly assembly, string resources)
        {
            return builder.UseStaticResources(new StaticResourcesSettings
            {
                Assembly = assembly,
                Resources = resources
            });
        }

        /// <summary>
        /// Use static files from resources of <paramref name="assembly"/> and filtered by <paramref name="resources"/>
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="assembly">Assembly with resources</param>
        /// <param name="resources">Resources filter</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder MapStaticResources(this IApplicationBuilder builder, string pathMatch, Assembly assembly, string resources)
        {
            return Map(builder, pathMatch, app => app.UseStaticResources(assembly, resources));
        }

        /// <summary>
        /// Use static files from resources
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="settings">Settings</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder UseStaticResources(this IApplicationBuilder builder, StaticResourcesSettings settings)
        {
            return builder.UseMiddleware<AssemblyResourcesMiddleware>(settings ?? new StaticResourcesSettings());
        }

        /// <summary>
        /// Use static files from resources
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="pathMatch">Custom path</param>
        /// <param name="settings">Settings</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder MapStaticResources(this IApplicationBuilder builder, string pathMatch, StaticResourcesSettings settings)
        {
            return Map(builder, pathMatch, app => app.UseStaticResources(settings));
        }

        /// <summary>
        /// Configures Ormikon Static default middleware
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Services collection</returns>
        public static IServiceCollection ConfigureStatic(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<StaticOptions>(configuration.GetSection("OrmikonStatic"));
        }

        /// <summary>
        /// Configures Ormikon Static default middleware
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="configurationSection">Configuration section</param>
        /// <returns>Services collection</returns>
        public static IServiceCollection ConfigureStatic(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            return services.Configure<StaticOptions>(configurationSection);
        }

        /// <summary>
        /// Configures Ormikon Static default middleware
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="configureOptions">Action to configure default static options</param>
        /// <returns>Services collection</returns>
        public static IServiceCollection ConfigureStatic(this IServiceCollection services, Action<StaticOptions> configureOptions)
        {
            return services.Configure(configureOptions);
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
