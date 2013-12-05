using System.Collections.Generic;
using System.Threading;

namespace Ormikon.Owin.Static.Wrappers
{
    internal interface IOwinContext
    {
        string Version { get; }

        CancellationToken CallCancelled { get; }

        IOwinRequest Request { get; }

        IOwinResponse Response { get; }

        IOwinSsl Ssl { get; }

        IOwinHost Host { get; }

        IOwinServer Server { get; }

        IDictionary<string, object> Environment { get; }
    }
}
