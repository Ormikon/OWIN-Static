using Ormikon.Owin.Static.Wrappers.Headers;
using System.Collections.Generic;
using System.IO;

namespace Ormikon.Owin.Static.Wrappers
{
    internal interface IOwinRequest
    {
        Stream Body { get; }

        IHttpRequestHeaders Headers { get; }

        string Method { get; }

        string Path { get; set; }

        string PathBase { get; set; }

        Location Location { get; }

        string Protocol { get; }

        string QueryString { get; }

        string Scheme { get; }
    }
}
