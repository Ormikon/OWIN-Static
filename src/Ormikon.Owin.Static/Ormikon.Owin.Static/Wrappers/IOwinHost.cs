using System.Collections.Generic;
using System.IO;

namespace Ormikon.Owin.Static.Wrappers
{
    internal interface IOwinHost
    {
        TextWriter TraceOutput { get; }

        IList<IDictionary<string, object>> Addresses { get; }
    }
}

