using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal abstract class HttpGeneralHeaders : HttpHeaders, IHttpGeneralHeaders
    {
        private HttpCacheControlHeader cacheControl;
        private HttpStringHeader connection;
        private HttpEnumHeader contentDisposition;
        private HttpStringHeader contentEncoding;
        private HttpEnumHeader contentLanguage;
        private HttpLongHeader contentLength;
        private HttpStringHeader contentLocation;
        private HttpStringHeader contentMd5;
        private HttpContentRangeHeader contentRange;
        private HttpPropertyHeader contentType;
        private HttpDateHeader date;
        private HttpDateHeader expires;
        private HttpDateHeader lastModified;
        private HttpStringHeader link;
        private HttpStringHeader title;
        private HttpEnumHeader trailer;
        private HttpEnumHeader transferEncoding;
        private HttpEnumHeader upgrade;
        private HttpEnumHeader via;
        private HttpStringHeader warning;

        protected HttpGeneralHeaders()
        {
        }

        protected HttpGeneralHeaders(IDictionary<string, StringValues> internalHeaders)
            : base(internalHeaders)
        {

        }

        public HttpCacheControlHeader CacheControl
        {
            get { return cacheControl ?? (cacheControl = new HttpCacheControlHeader(this)); }
        }

        public HttpStringHeader Connection
        {
            get { return connection ?? (connection = new HttpStringHeader(this, Constants.Http.Headers.Connection)); }
        }

        public HttpEnumHeader ContentDisposition
        {
            get { return contentDisposition ?? (contentDisposition = new HttpEnumHeader(this, Constants.Http.Headers.ContentDisposition)); }
        }

        public HttpStringHeader ContentEncoding
        {
            get { return contentEncoding ?? (contentEncoding = new HttpStringHeader(this, Constants.Http.Headers.ContentEncoding)); }
        }

        public HttpEnumHeader ContentLanguage
        {
            get { return contentLanguage ?? (contentLanguage = new HttpEnumHeader(this, Constants.Http.Headers.ContentLanguage)); }
        }

        public HttpLongHeader ContentLength
        {
            get { return contentLength ?? (contentLength = new HttpLongHeader(this, Constants.Http.Headers.ContentLength)); }
        }

        public HttpStringHeader ContentLocation
        {
            get { return contentLocation ?? (contentLocation = new HttpStringHeader(this, Constants.Http.Headers.ContentLocation)); }
        }

        public HttpStringHeader ContentMd5
        {
            get { return contentMd5 ?? (contentMd5 = new HttpStringHeader(this, Constants.Http.Headers.ContentMd5)); }
        }

        public HttpContentRangeHeader ContentRange
        {
            get { return contentRange ?? (contentRange = new HttpContentRangeHeader(this)); }
        }

        public HttpPropertyHeader ContentType
        {
            get { return contentType ?? (contentType = new HttpPropertyHeader(this, Constants.Http.Headers.ContentType)); }
        }

        public HttpDateHeader Date
        {
            get { return date ?? (date = new HttpDateHeader(this, Constants.Http.Headers.Date)); }
        }

        public HttpDateHeader Expires
        {
            get { return expires ?? (expires = new HttpDateHeader(this, Constants.Http.Headers.Expires)); }
        }

        public HttpDateHeader LastModified
        {
            get { return lastModified ?? (lastModified = new HttpDateHeader(this, Constants.Http.Headers.LastModified)); }
        }

        public HttpStringHeader Link
        {
            get { return link ?? (link = new HttpStringHeader(this, Constants.Http.Headers.Link)); }
        }

        public HttpStringHeader Title
        {
            get { return title ?? (title = new HttpStringHeader(this, Constants.Http.Headers.Title)); }
        }

        public HttpEnumHeader Trailer
        {
            get { return trailer ?? (trailer = new HttpEnumHeader(this, Constants.Http.Headers.Trailer)); }
        }

        public HttpEnumHeader TransferEncoding
        {
            get { return transferEncoding ?? (transferEncoding = new HttpEnumHeader(this, Constants.Http.Headers.TransferEncoding)); }
        }

        public HttpEnumHeader Upgrade
        {
            get { return upgrade ?? (upgrade = new HttpEnumHeader(this, Constants.Http.Headers.Upgrade)); }
        }

        public HttpEnumHeader Via
        {
            get { return via ?? (via = new HttpEnumHeader(this, Constants.Http.Headers.Via)); }
        }

        public HttpStringHeader Warning
        {
            get { return warning ?? (warning = new HttpStringHeader(this, Constants.Http.Headers.Warning)); }
        }
    }
}
