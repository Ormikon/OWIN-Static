using System;
using System.Collections.Generic;
using System.Reflection;
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

        protected override StaticResponse GetResponse(Location location)
        {
            throw new NotImplementedException();
        }
    }
}
