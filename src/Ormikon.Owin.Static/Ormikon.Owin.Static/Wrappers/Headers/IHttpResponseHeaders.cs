using System;

namespace Ormikon.Owin.Static.Headers
{
    internal interface IHttpResponseHeaders : IHttpHeaders
    {
        HttpAcceptHeader AcceptRanges { get; }
        HttpLongHeader Age { get; }
        HttpEnumHeader Allow { get; }
        HttpCacheControlHeader CacheControl { get; }
        HttpEnumHeader ContentDisposition { get; }
        HttpStringHeader ContentEncoding { get; }
        HttpEnumHeader ContentLanguage { get; }
        HttpLongHeader ContentLength { get; }
        HttpStringHeader ContentLocation { get; }
        HttpStringHeader ContentMd5 { get; }
    }
}

