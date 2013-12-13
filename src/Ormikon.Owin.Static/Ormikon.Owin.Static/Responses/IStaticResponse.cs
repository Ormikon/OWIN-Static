using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.Responses
{
    interface IStaticResponse
    {
        int StatusCode { get; }
        string ReasonPhrase { get; }

        IHttpResponseHeaders Headers { get; }
    }
}
