using System;

namespace Ormikon.Owin.Static.Wrappers.Headers
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
        HttpContentRangeHeader ContentRange { get; }
        HttpPropertyHeader ContentType { get; }
        HttpPropertyHeader Cookie { get; }
        HttpDateHeader Date { get; }
        HttpStringHeader Expect { get; }
        HttpDateHeader Expires { get; }
        HttpStringHeader From { get; }
        HttpStringHeader Host { get; }
        HttpEnumHeader IfMatch { get; }
        HttpDateHeader IfModifiedSince { get; }
        HttpEnumHeader IfNoneMatch { get; }
        HttpIfRangeHeader IfRange { get; }
        HttpDateHeader IfUnmodifiedSince { get; }
        HttpDateHeader LastModified { get; }
        HttpStringHeader Link { get; }
        HttpLongHeader MaxForwards { get; }
        HttpStringHeader Pragma { get; }
        HttpStringHeader ProxyAuthenticate { get; }
        HttpStringHeader ProxyAuthorization { get; }
        HttpRangeHeader Range { get; }
        HttpStringHeader Referer { get; }
        HttpStringHeader Title { get; }
        HttpEnumHeader TE { get; }
        HttpEnumHeader Trailer { get; }
        HttpEnumHeader TransferEncoding { get; }
        HttpEnumHeader Upgrade { get; }
        HttpStringHeader UserAgent { get; }
        HttpEnumHeader Via { get; }
        HttpStringHeader Warning { get; }
    }
}

