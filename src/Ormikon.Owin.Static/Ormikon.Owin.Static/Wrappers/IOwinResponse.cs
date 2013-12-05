using System.Collections.Generic;
using System.IO;

namespace Ormikon.Owin.Static.Wrappers
{
    internal interface IOwinResponse
    {
        Stream Body { get; }

        IDictionary<string, string[]> Headers { get; }

        int StatusCode { get; set; }

        string ReasonPhrase { get; set; }

        string Protocol { get; set; }
    }
}
