using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
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

        public HttpResponseHeaders(IDictionary<string, StringValues> internalHeaders)
            : base(internalHeaders)
        {
        }

        protected override IHttpHeaders CreateInstance()
        {
            return new HttpResponseHeaders();
        }

        public HttpAcceptHeader AcceptRanges => acceptRanges ?? (acceptRanges = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptRanges));

        public HttpLongHeader Age => age ?? (age = new HttpLongHeader(this, Constants.Http.Headers.Age));

        public HttpEnumHeader Allow => allow ?? (allow = new HttpEnumHeader(this, Constants.Http.Headers.Allow));

        public HttpStringHeader ETag => eTag ?? (eTag = new HttpStringHeader(this, Constants.Http.Headers.ETag));

        public HttpStringHeader Location => location ?? (location = new HttpStringHeader(this, Constants.Http.Headers.Location));

        public HttpStringHeader ProxyAuthenticate => proxyAuthenticate ?? (proxyAuthenticate = new HttpStringHeader(this, Constants.Http.Headers.ProxyAuthenticate));

        public HttpEnumHeader Public => @public ?? (@public = new HttpEnumHeader(this, Constants.Http.Headers.Public));

        public HttpRetryAfterHeader RetryAfter => retryAfter ?? (retryAfter = new HttpRetryAfterHeader(this));

        public HttpStringHeader Server => server ?? (server = new HttpStringHeader(this, Constants.Http.Headers.Server));

        public HttpPropertyHeader SetCookie => setCookie ?? (setCookie = new HttpPropertyHeader(this, Constants.Http.Headers.SetCookie));

        public HttpEnumHeader Vary => vary ?? (vary = new HttpEnumHeader(this, Constants.Http.Headers.Vary));

        public HttpStringHeader WwwAuthenticate => wwwAuthenticate ?? (wwwAuthenticate = new HttpStringHeader(this, Constants.Http.Headers.WwwAuthenticate));
    }
}
