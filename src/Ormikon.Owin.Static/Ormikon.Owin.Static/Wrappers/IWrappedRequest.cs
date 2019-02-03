using Ormikon.Owin.Static.Wrappers.Headers;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Ormikon.Owin.Static.Wrappers
{
    internal interface IWrappedRequest
    {
        Stream Body { get; }

        IHttpRequestHeaders Headers { get; }

        string Method { get; }

        string Path { get; set; }

        string PathBase { get; set; }

        Location Location { get; set; }

        string Protocol { get; }

        QueryString QueryString { get; }

        string Scheme { get; }
    }
}
