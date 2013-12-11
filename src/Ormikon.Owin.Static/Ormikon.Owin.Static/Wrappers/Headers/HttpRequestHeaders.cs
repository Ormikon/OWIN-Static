using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpRequestHeaders : HttpHeaders, IHttpRequestHeaders
    {
        public HttpRequestHeaders()
        {
            InitializeHeaders();
        }

        public HttpRequestHeaders(IDictionary<string, string[]> headers)
            : base(headers)
        {
            InitializeHeaders();
        }

        private void InitializeHeaders()
        {
            Accept = new HttpAcceptHeader(this, Constants.Http.Headers.Accept);
            AcceptCharset = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptCharset);
            AcceptEncoding = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptEncoding);
            AcceptLanguage = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptLanguage);
            Authorization = new HttpStringHeader(this, Constants.Http.Headers.AcceptCharset);
            CacheControl = new HttpCacheControlHeader(this);
            Connection = new HttpStringHeader(this, Constants.Http.Headers.Connection);
            ContentDisposition = new HttpEnumHeader(this, Constants.Http.Headers.ContentDisposition);
            ContentEncoding = new HttpStringHeader(this, Constants.Http.Headers.ContentEncoding);
            ContentLanguage = new HttpEnumHeader(this, Constants.Http.Headers.ContentLanguage);
            ContentLength = new HttpLongHeader(this, Constants.Http.Headers.ContentLength);
            ContentLocation = new HttpStringHeader(this, Constants.Http.Headers.ContentLocation);
            ContentMd5 = new HttpStringHeader(this, Constants.Http.Headers.ContentMd5);
            ContentRange = new HttpContentRangeHeader(this);
            ContentType = new HttpPropertyHeader(this, Constants.Http.Headers.ContentType);
            Cookie = new HttpPropertyHeader(this, Constants.Http.Headers.Cookie);
            Date = new HttpDateHeader(this, Constants.Http.Headers.Date);
            Expect = new HttpStringHeader(this, Constants.Http.Headers.Expect);
            Expires = new HttpDateHeader(this, Constants.Http.Headers.Expires);
            From = new HttpStringHeader(this, Constants.Http.Headers.From);
            Host = new HttpStringHeader(this, Constants.Http.Headers.Host);
            IfMatch = new HttpEnumHeader(this, Constants.Http.Headers.IfMatch);
            IfModifiedSince = new HttpDateHeader(this, Constants.Http.Headers.IfModifiedSince);
            IfNoneMatch = new HttpEnumHeader(this, Constants.Http.Headers.IfNoneMatch);
            IfRange = new HttpIfRangeHeader(this);
            IfUnmodifiedSince = new HttpDateHeader(this, Constants.Http.Headers.IfUnmodifiedSince);
            LastModified = new HttpDateHeader(this, Constants.Http.Headers.IfUnmodifiedSince);
            Link = new HttpStringHeader(this, Constants.Http.Headers.Link);
            MaxForwards = new HttpLongHeader(this, Constants.Http.Headers.MaxForwards);
            Pragma = new HttpStringHeader(this, Constants.Http.Headers.Pragma);
            ProxyAuthenticate = new HttpStringHeader(this, Constants.Http.Headers.ProxyAuthenticate);
            ProxyAuthorization = new HttpStringHeader(this, Constants.Http.Headers.ProxyAuthorization);
            Range = new HttpRangeHeader(this);
            Referer = new HttpStringHeader(this, Constants.Http.Headers.Referer);
            Title = new HttpStringHeader(this, Constants.Http.Headers.Title);
            TE = new HttpEnumHeader(this, Constants.Http.Headers.TE);
            Trailer = new HttpEnumHeader(this, Constants.Http.Headers.Trailer);
            TransferEncoding = new HttpEnumHeader(this, Constants.Http.Headers.TransferEncoding);
            Upgrade = new HttpEnumHeader(this, Constants.Http.Headers.Upgrade);
            UserAgent = new HttpStringHeader(this, Constants.Http.Headers.UserAgent);
            Via = new HttpEnumHeader(this, Constants.Http.Headers.Via);
            Warning = new HttpStringHeader(this, Constants.Http.Headers.Warning);
        }

        #region IHttpRequestHeaders Members

        public HttpAcceptHeader Accept
        {
            get;
            private set;
        }

        public HttpAcceptHeader AcceptCharset
        {
            get;
            private set;
        }

        public HttpAcceptHeader AcceptEncoding
        {
            get;
            private set;
        }

        public HttpAcceptHeader AcceptLanguage
        {
            get;
            private set;
        }

        public HttpStringHeader Authorization
        {
            get;
            private set;
        }

        public HttpCacheControlHeader CacheControl
        {
            get;
            private set;
        }

        public HttpStringHeader Connection
        {
            get;
            private set;
        }

        public HttpEnumHeader ContentDisposition
        {
            get;
            private set;
        }

        public HttpStringHeader ContentEncoding
        {
            get;
            private set;
        }

        public HttpEnumHeader ContentLanguage
        {
            get;
            private set;
        }

        public HttpLongHeader ContentLength
        {
            get;
            private set;
        }

        public HttpStringHeader ContentLocation
        {
            get;
            private set;
        }

        public HttpStringHeader ContentMd5
        {
            get;
            private set;
        }

        public HttpContentRangeHeader ContentRange
        {
            get;
            private set;
        }

        public HttpPropertyHeader ContentType
        {
            get;
            private set;
        }

        public HttpPropertyHeader Cookie
        {
            get;
            private set;
        }

        public HttpDateHeader Date
        {
            get;
            private set;
        }

        public HttpStringHeader Expect
        {
            get;
            private set;
        }

        public HttpDateHeader Expires
        {
            get;
            private set;
        }

        public HttpStringHeader From
        {
            get;
            private set;
        }

        public HttpStringHeader Host
        {
            get;
            private set;
        }

        public HttpEnumHeader IfMatch
        {
            get;
            private set;
        }

        public HttpDateHeader IfModifiedSince
        {
            get;
            private set;
        }

        public HttpEnumHeader IfNoneMatch
        {
            get;
            private set;
        }

        public HttpIfRangeHeader IfRange
        {
            get;
            private set;
        }

        public HttpDateHeader IfUnmodifiedSince
        {
            get;
            private set;
        }

        public HttpDateHeader LastModified
        {
            get;
            private set;
        }

        public HttpStringHeader Link
        {
            get;
            private set;
        }

        public HttpLongHeader MaxForwards
        {
            get;
            private set;
        }

        public HttpStringHeader Pragma
        {
            get;
            private set;
        }

        public HttpStringHeader ProxyAuthenticate
        {
            get;
            private set;
        }

        public HttpStringHeader ProxyAuthorization
        {
            get;
            private set;
        }

        public HttpRangeHeader Range
        {
            get;
            private set;
        }

        public HttpStringHeader Referer
        {
            get;
            private set;
        }

        public HttpStringHeader Title
        {
            get;
            private set;
        }

        public HttpEnumHeader TE
        {
            get;
            private set;
        }

        public HttpEnumHeader Trailer
        {
            get;
            private set;
        }

        public HttpEnumHeader TransferEncoding
        {
            get;
            private set;
        }

        public HttpEnumHeader Upgrade
        {
            get;
            private set;
        }

        public HttpStringHeader UserAgent
        {
            get;
            private set;
        }

        public HttpEnumHeader Via
        {
            get;
            private set;
        }

        public HttpStringHeader Warning
        {
            get;
            private set;
        }

        #endregion

        protected override IHttpHeaders CreateInstance()
        {
            return new HttpRequestHeaders();
        }
    }
}
