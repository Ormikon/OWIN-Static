using Ormikon.Owin.Static.Wrappers.Headers;
using System.IO;

namespace Ormikon.Owin.Static.Wrappers
{
    internal interface IWrappedResponse
    {
        Stream Body { get; }

        IHttpResponseHeaders Headers { get; }

        int StatusCode { get; set; }

        string ReasonPhrase { get; set; }
    }
}
