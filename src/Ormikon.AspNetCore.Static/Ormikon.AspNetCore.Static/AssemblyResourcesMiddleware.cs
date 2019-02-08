using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static
{
    internal class AssemblyResourcesMiddleware : StaticMiddlewareBase
    {
        public AssemblyResourcesMiddleware(RequestDelegate next, Assembly assembly, string resource)
            : base(next, true)
        {
        }

        protected override Task<StaticResponse>
            GetResponseAsync(Location location, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
