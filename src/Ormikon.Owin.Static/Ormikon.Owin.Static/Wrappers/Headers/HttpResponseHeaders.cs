using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpResponseHeaders : HttpHeaders, IHttpResponseHeaders
    {
        public HttpResponseHeaders()
        {
            InitializeHeaders();
        }

        public HttpResponseHeaders(IDictionary<string, string[]> headers)
            : base(headers)
        {
            InitializeHeaders();
        }

        private void InitializeHeaders()
        {
            AcceptRanges = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptRanges);
            Age = new HttpLongHeader(this, Constants.Http.Headers.Age);
            Allow = new HttpEnumHeader(this, Constants.Http.Headers.Allow);
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
            Date = new HttpDateHeader(this, Constants.Http.Headers.Date);
            ETag = new HttpStringHeader(this, Constants.Http.Headers.ETag);
            Expires = new HttpDateHeader(this, Constants.Http.Headers.Expires);
            LastModified = new HttpDateHeader(this, Constants.Http.Headers.LastModified);
            Link = new HttpStringHeader(this, Constants.Http.Headers.Link);
            Location = new HttpStringHeader(this, Constants.Http.Headers.Location);
            ProxyAuthenticate = new HttpStringHeader(this, Constants.Http.Headers.ProxyAuthenticate);
            Public = new HttpEnumHeader(this, Constants.Http.Headers.Public);
            RetryAfter = new HttpRetryAfterHeader(this);
            Server = new HttpStringHeader(this, Constants.Http.Headers.Server);
            SetCookie = new HttpPropertyHeader(this, Constants.Http.Headers.SetCookie);
            Title = new HttpStringHeader(this, Constants.Http.Headers.Title);
            Trailer = new HttpEnumHeader(this, Constants.Http.Headers.Trailer);
            TransferEncoding = new HttpEnumHeader(this, Constants.Http.Headers.TransferEncoding);
            Upgrade = new HttpEnumHeader(this, Constants.Http.Headers.Upgrade);
            Vary = new HttpEnumHeader(this, Constants.Http.Headers.Vary);
            Via = new HttpEnumHeader(this, Constants.Http.Headers.Via);
            Warning = new HttpStringHeader(this, Constants.Http.Headers.Warning);
            WwwAuthenticate = new HttpStringHeader(this, Constants.Http.Headers.WwwAuthenticate);

        }

        #region IHttpResponseHeaders Members

        public HttpAcceptHeader AcceptRanges
        {
            get;
            private set;
        }

        public HttpLongHeader Age
        {
            get;
            private set;
        }

        public HttpEnumHeader Allow
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

        public HttpDateHeader Date
        {
            get;
            private set;
        }

        public HttpStringHeader ETag
        {
            get;
            private set;
        }

        public HttpDateHeader Expires
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

        public HttpStringHeader Location
        {
            get;
            private set;
        }

        public HttpStringHeader ProxyAuthenticate
        {
            get;
            private set;
        }

        public HttpEnumHeader Public
        {
            get;
            private set;
        }

        public HttpRetryAfterHeader RetryAfter
        {
            get;
            private set;
        }

        public HttpStringHeader Server
        {
            get;
            private set;
        }

        public HttpPropertyHeader SetCookie
        {
            get;
            private set;
        }

        public HttpStringHeader Title
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

        public HttpEnumHeader Vary
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

        public HttpStringHeader WwwAuthenticate
        {
            get;
            private set;
        }

        #endregion

        protected override IHttpHeaders CreateInstance()
        {
            return new HttpResponseHeaders();
        }
    }
}
