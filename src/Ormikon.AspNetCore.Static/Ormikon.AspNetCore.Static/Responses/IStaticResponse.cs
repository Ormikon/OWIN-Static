using Ormikon.AspNetCore.Static.Wrappers.Headers;

namespace Ormikon.AspNetCore.Static.Responses
{
    interface IStaticResponse
    {
        int StatusCode { get; }
        string ReasonPhrase { get; }

        IHttpResponseHeaders Headers { get; }
    }
}
