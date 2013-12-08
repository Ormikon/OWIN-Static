using System;

namespace Ormikon.Owin.Static.Headers
{
    internal interface IHttpRequestHeaders : IHttpHeaders
    {
        HttpAcceptHeader Accept { get; }
        HttpAcceptHeader AcceptCharset { get; }
        HttpAcceptHeader AcceptEncoding { get; }
        HttpAcceptHeader AcceptLanguage { get; }
        HttpStringHeader Authorization { get; }
        HttpCacheControlHeader CacheControl { get; }
        HttpStringHeader Connection { get; }
        HttpEnumHeader ContentDisposition { get; }
        HttpStringHeader ContentEncoding { get; }
        HttpEnumHeader ContentLanguage { get; }
        HttpLongHeader ContentLength { get; }
        HttpStringHeader ContentLocation { get; }
        HttpStringHeader ContentMd5 { get; }
    }
}

