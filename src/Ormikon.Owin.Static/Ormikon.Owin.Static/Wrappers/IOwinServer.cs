using System;
using System.Net;
using System.Collections.Generic;

namespace Ormikon.Owin.Static
{
    internal interface IOwinServer
    {
        IPAddress RemoteIpAddress{ get; }

        int RemotePort { get; }

        IPAddress LocalIpAddress { get; }

        int LocalPort { get; }

        bool IsLocal { get; }

        IDictionary<string, object> Capabilities { get; }

        Action<Action<object>, object> OnSendingHeaders { get; }
    }
}

