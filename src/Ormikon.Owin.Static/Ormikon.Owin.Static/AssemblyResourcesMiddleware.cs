﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;

namespace Ormikon.Owin.Static
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
