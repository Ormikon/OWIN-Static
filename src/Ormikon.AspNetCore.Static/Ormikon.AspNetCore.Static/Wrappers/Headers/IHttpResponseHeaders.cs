namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal interface IHttpResponseHeaders : IHttpGeneralHeaders
    {
        HttpAcceptHeader AcceptRanges { get; }
        HttpLongHeader Age { get; }
        HttpEnumHeader Allow { get; }
        HttpStringHeader ETag { get; }
        HttpStringHeader Location { get; }
        HttpStringHeader ProxyAuthenticate { get; }
        HttpEnumHeader Public { get; }
        HttpRetryAfterHeader RetryAfter { get; }
        HttpStringHeader Server { get; }
        HttpPropertyHeader SetCookie { get; }
        HttpEnumHeader Vary { get; }
        HttpStringHeader WwwAuthenticate { get; }
    }
}

