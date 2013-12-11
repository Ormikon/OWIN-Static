using Ormikon.Owin.Static.Wrappers.Headers;
using System.Collections.Generic;
using System.IO;

namespace Ormikon.Owin.Static.Wrappers
{
    internal interface IOwinResponse
    {
        Stream Body { get; }

        IHttpResponseHeaders Headers { get; }

        int StatusCode { get; set; }

        string ReasonPhrase { get; set; }

        string Protocol { get; set; }
    }
}
