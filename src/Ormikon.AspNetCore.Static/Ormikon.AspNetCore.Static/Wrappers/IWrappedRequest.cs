using System.IO;
using Microsoft.AspNetCore.Http;
using Ormikon.AspNetCore.Static.Wrappers.Headers;

namespace Ormikon.AspNetCore.Static.Wrappers
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
