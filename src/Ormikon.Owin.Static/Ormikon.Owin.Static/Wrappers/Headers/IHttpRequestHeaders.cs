namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal interface IHttpRequestHeaders : IHttpGeneralHeaders
    {
        HttpAcceptHeader Accept { get; }
        HttpAcceptHeader AcceptCharset { get; }
        HttpAcceptHeader AcceptEncoding { get; }
        HttpAcceptHeader AcceptLanguage { get; }
        HttpStringHeader Authorization { get; }
        HttpPropertyHeader Cookie { get; }
        HttpStringHeader Expect { get; }
        HttpStringHeader From { get; }
        HttpStringHeader Host { get; }
        HttpEnumHeader IfMatch { get; }
        HttpDateHeader IfModifiedSince { get; }
        HttpEnumHeader IfNoneMatch { get; }
        HttpIfRangeHeader IfRange { get; }
        HttpDateHeader IfUnmodifiedSince { get; }
        HttpLongHeader MaxForwards { get; }
        HttpStringHeader Pragma { get; }
        HttpStringHeader ProxyAuthorization { get; }
        HttpRangeHeader Range { get; }
        HttpStringHeader Referer { get; }
        HttpEnumHeader Te { get; }
        HttpStringHeader UserAgent { get; }
    }
}

