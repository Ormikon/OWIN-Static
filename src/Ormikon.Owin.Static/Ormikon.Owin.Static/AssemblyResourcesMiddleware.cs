using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Ormikon.Owin.Static
{
    internal class AssemblyResourcesMiddleware : StaticMiddlewareBase
    {
        public AssemblyResourcesMiddleware(OwinMiddleware next, StaticSettings settings) : base(next, settings)
        {
        }

        protected override string ResolveResource(PathString path, out bool isFolder)
        {
            throw new NotImplementedException();
        }

        protected override Stream GetResourceStream(string path)
        {
            throw new NotImplementedException();
        }
    }
}
