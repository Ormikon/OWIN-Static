namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal interface IHttpGeneralHeaders : IHttpHeaders
    {
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
        HttpDateHeader Expires { get; }
        HttpDateHeader LastModified { get; }
        HttpStringHeader Link { get; }
        HttpStringHeader Title { get; }
        HttpEnumHeader Trailer { get; }
        HttpEnumHeader TransferEncoding { get; }
        HttpEnumHeader Upgrade { get; }
        HttpEnumHeader Via { get; }
        HttpStringHeader Warning { get; }
    }
}
