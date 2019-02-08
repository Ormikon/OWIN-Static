using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Ormikon.AspNetCore.Static.Mapping;
using Ormikon.AspNetCore.Static.Options;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static
{
    internal class StaticOptionsMiddleware
    {
        private readonly BaseMiddleware middleware;

        public StaticOptionsMiddleware(RequestDelegate next, IOptions<StaticOptions> options, IHostingEnvironment hostEnvironment)
        {
            var staticOptions = options?.Value ?? new StaticOptions();

            if (!staticOptions.HasMappings())
            {
                staticOptions = new StaticOptions
                {
                    Maps = new[] {new MapOptions {Path = null, Sources = null}}
                };
            }

            middleware = OptionsToMiddleware(staticOptions, next, hostEnvironment).Last();
        }

        private static IEnumerable<BaseMiddleware> OptionsToMiddleware(StaticOptions staticOptions, RequestDelegate next,
            IHostingEnvironment hostEnvironment)
        {
            foreach (var mapOptions in staticOptions.Maps)
            {
                var m = OptionsToMiddleware(mapOptions, next, hostEnvironment);
                next = m.Invoke;
                yield return m;
            }
        }

        private static BaseMiddleware OptionsToMiddleware(MapOptions mapOptions, RequestDelegate next,
            IHostingEnvironment hostEnvironment)
        {
            BaseMiddleware m = new StaticMiddleware(next, mapOptions.CreateSettings(), hostEnvironment);

            if (!string.IsNullOrEmpty(mapOptions.Path))
            {
                m = new MapMiddleware(next, mapOptions.Path, m.Invoke);
            }

            return m;
        }

        public Task Invoke(HttpContext context) => middleware.Invoke(context);
    }
}
