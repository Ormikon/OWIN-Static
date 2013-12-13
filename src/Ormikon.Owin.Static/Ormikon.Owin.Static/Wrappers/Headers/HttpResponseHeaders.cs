using System.Collections.Generic;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpResponseHeaders : HttpGeneralHeaders, IHttpResponseHeaders
    {
        private HttpAcceptHeader acceptRanges;
        private HttpLongHeader age;
        private HttpEnumHeader allow;
        private HttpStringHeader eTag;
        private HttpStringHeader location;
        private HttpStringHeader proxyAuthenticate;
        private HttpEnumHeader @public;
        private HttpRetryAfterHeader retryAfter;
        private HttpStringHeader server;
        private HttpPropertyHeader setCookie;
        private HttpEnumHeader vary;
        private HttpStringHeader wwwAuthenticate;

        public HttpResponseHeaders()
        {
        }

        public HttpResponseHeaders(IDictionary<string, string[]> internalHeaders)
            : base(internalHeaders)
        {
        }

        protected override IHttpHeaders CreateInstance()
        {
            return new HttpResponseHeaders();
        }

        public HttpAcceptHeader AcceptRanges
        {
            get { return acceptRanges ?? (acceptRanges = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptRanges)); }
        }

        public HttpLongHeader Age
        {
            get { return age ?? (age = new HttpLongHeader(this, Constants.Http.Headers.Age)); }
        }

        public HttpEnumHeader Allow
        {
            get { return allow ?? (allow = new HttpEnumHeader(this, Constants.Http.Headers.Allow)); }
        }

        public HttpStringHeader ETag
        {
            get { return eTag ?? (eTag = new HttpStringHeader(this, Constants.Http.Headers.ETag)); }
        }

        public HttpStringHeader Location
        {
            get { return location ?? (location = new HttpStringHeader(this, Constants.Http.Headers.Location)); }
        }

        public HttpStringHeader ProxyAuthenticate
        {
            get { return proxyAuthenticate ?? (proxyAuthenticate = new HttpStringHeader(this, Constants.Http.Headers.ProxyAuthenticate)); }
        }

        public HttpEnumHeader Public
        {
            get { return @public ?? (@public = new HttpEnumHeader(this, Constants.Http.Headers.Public)); }
        }

        public HttpRetryAfterHeader RetryAfter
        {
            get { return retryAfter ?? (retryAfter = new HttpRetryAfterHeader(this)); }
        }

        public HttpStringHeader Server
        {
            get { return server ?? (server = new HttpStringHeader(this, Constants.Http.Headers.Server)); }
        }

        public HttpPropertyHeader SetCookie
        {
            get { return setCookie ?? (setCookie = new HttpPropertyHeader(this, Constants.Http.Headers.SetCookie)); }
        }

        public HttpEnumHeader Vary
        {
            get { return vary ?? (vary = new HttpEnumHeader(this, Constants.Http.Headers.Vary)); }
        }

        public HttpStringHeader WwwAuthenticate
        {
            get { return wwwAuthenticate ?? (wwwAuthenticate = new HttpStringHeader(this, Constants.Http.Headers.WwwAuthenticate)); }
        }
    }
}
