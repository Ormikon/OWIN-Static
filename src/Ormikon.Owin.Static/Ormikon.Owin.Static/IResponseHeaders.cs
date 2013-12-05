using System.Collections.Generic;

namespace Ormikon.Owin.Static
{
    interface IResponseHeaders
    {
        int StatusCode { get; }

        IDictionary<string, string[]> Headers { get; }
    }
}
