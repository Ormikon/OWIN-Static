using System;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal interface IHttpResponseHeaders : IHttpHeaders
    {
        HttpAcceptHeader AcceptRanges { get; }
        HttpLongHeader Age { get; }
        HttpEnumHeader Allow { get; }
        HttpCacheControlHeader CacheControl { get; }
        HttpStringHeader Connection { get; }
        HttpEnumHeader ContentDisposition { get; }
        HttpStringHeader ContentEncoding { get; }
        HttpEnumHeader ContentLanguage { get; }
        HttpLongHeader ContentLength { get; }
        HttpStringHeader ContentLocation { get; }
        HttpStringHeader ContentMd5 { get; }
        HttpContentRangeHeader ContentRange { get; }
        HttpPropertyHeader ContentType { get; }
        HttpDateHeader Date { get; }
        HttpStringHeader ETag { get; }
        HttpDateHeader Expires { get; }
        HttpDateHeader LastModified { get; }
        HttpStringHeader Link { get; }
        HttpStringHeader Location { get; }
        HttpStringHeader ProxyAuthenticate { get; }
        HttpEnumHeader Public { get; }
        HttpRetryAfterHeader RetryAfter { get; }
        HttpStringHeader Server { get; }
        HttpPropertyHeader SetCookie { get; }
        HttpStringHeader Title { get; }
        HttpEnumHeader Trailer { get; }
        HttpEnumHeader TransferEncoding { get; }
        HttpEnumHeader Upgrade { get; }
        HttpEnumHeader Vary { get; }
        HttpEnumHeader Via { get; }
        HttpStringHeader Warning { get; }
        HttpStringHeader WwwAuthenticate { get; }
    }
}

