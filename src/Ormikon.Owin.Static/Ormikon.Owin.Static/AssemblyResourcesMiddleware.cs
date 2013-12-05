using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static
{
    internal class AssemblyResourcesMiddleware : StaticMiddlewareBase
    {
        public AssemblyResourcesMiddleware(Func<IDictionary<string, object>, Task> next, Assembly assembly, string resource)
            : base(next, true)
        {
        }

        protected override StaticResponse GetResponse(Location location)
        {
            throw new NotImplementedException();
        }
    }
}
