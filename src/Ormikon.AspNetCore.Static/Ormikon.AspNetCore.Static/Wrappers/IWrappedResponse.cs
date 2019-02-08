using System.IO;
using Ormikon.AspNetCore.Static.Wrappers.Headers;

namespace Ormikon.AspNetCore.Static.Wrappers
{
    internal interface IWrappedResponse
    {
        Stream Body { get; }

        IHttpResponseHeaders Headers { get; }

        int StatusCode { get; set; }

        string ReasonPhrase { get; set; }
    }
}
