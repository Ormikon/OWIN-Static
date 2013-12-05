using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Mapping;
using Owin;

namespace Ormikon.Owin.Static.Extensions
{
    internal static class MapExtensions
    {
        public static IAppBuilder Map(this IAppBuilder builder, string path, Action<IAppBuilder> configuration)
        {
            var newBuilder = builder.New();
            configuration(newBuilder);
            var chain =
                newBuilder.Build(typeof (Func<IDictionary<string, object>, Task>)) as
                Func<IDictionary<string, object>, Task>;
            return builder.Use(typeof(MapMiddleware), path, chain); ;
        }
    }
}
