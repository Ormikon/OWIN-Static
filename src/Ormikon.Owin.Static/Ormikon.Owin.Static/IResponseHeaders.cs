using System.Collections.Generic;

namespace Ormikon.Owin.Static
{
    interface IResponseHeaders
    {
        int Status { get; }

        IDictionary<string, string[]> Headers { get; }
    }
}
