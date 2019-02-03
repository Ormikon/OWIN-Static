using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpRequestHeaders : HttpGeneralHeaders, IHttpRequestHeaders
    {
        private HttpAcceptHeader accept;
        private HttpAcceptHeader acceptCharset;
        private HttpAcceptHeader acceptEncoding;
        private HttpAcceptHeader acceptLanguage;
        private HttpStringHeader authorization;
        private HttpPropertyHeader cookie;
        private HttpStringHeader expect;
        private HttpStringHeader @from;
        private HttpStringHeader host;
        private HttpEnumHeader ifMatch;
        private HttpDateHeader ifModifiedSince;
        private HttpEnumHeader ifNoneMatch;
        private HttpIfRangeHeader ifRange;
        private HttpDateHeader ifUnmodifiedSince;
        private HttpLongHeader maxForwards;
        private HttpStringHeader pragma;
        private HttpStringHeader proxyAuthorization;
        private HttpRangeHeader range;
        private HttpStringHeader referer;
        private HttpEnumHeader te;
        private HttpStringHeader userAgent;

        public HttpRequestHeaders()
        {
        }

        public HttpRequestHeaders(IDictionary<string, StringValues> internalHeaders)
            : base(internalHeaders)
        {

        }

        protected override IHttpHeaders CreateInstance()
        {
            return new HttpRequestHeaders();
        }

        public HttpAcceptHeader Accept
        {
            get { return accept ?? (accept = new HttpAcceptHeader(this, Constants.Http.Headers.Accept)); }
        }

        public HttpAcceptHeader AcceptCharset
        {
            get { return acceptCharset ?? (acceptCharset = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptCharset)); }
        }

        public HttpAcceptHeader AcceptEncoding
        {
            get { return acceptEncoding ?? (acceptEncoding = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptEncoding)); }
        }

        public HttpAcceptHeader AcceptLanguage
        {
            get { return acceptLanguage ?? (acceptLanguage = new HttpAcceptHeader(this, Constants.Http.Headers.AcceptLanguage)); }
        }

        public HttpStringHeader Authorization
        {
            get { return authorization ?? (authorization = new HttpStringHeader(this, Constants.Http.Headers.Authorization)); }
        }

        public HttpPropertyHeader Cookie
        {
            get { return cookie ?? (cookie = new HttpPropertyHeader(this, Constants.Http.Headers.Cookie)); }
        }

        public HttpStringHeader Expect
        {
            get { return expect ?? (expect = new HttpStringHeader(this, Constants.Http.Headers.Expect)); }
        }

        public HttpStringHeader From
        {
            get { return @from ?? (@from = new HttpStringHeader(this, Constants.Http.Headers.From)); }
        }

        public HttpStringHeader Host
        {
            get { return host ?? (host = new HttpStringHeader(this, Constants.Http.Headers.Host)); }
        }

        public HttpEnumHeader IfMatch
        {
            get { return ifMatch ?? (ifMatch = new HttpEnumHeader(this, Constants.Http.Headers.IfMatch)); }
        }

        public HttpDateHeader IfModifiedSince
        {
            get { return ifModifiedSince ?? (ifModifiedSince = new HttpDateHeader(this, Constants.Http.Headers.IfModifiedSince)); }
        }

        public HttpEnumHeader IfNoneMatch
        {
            get { return ifNoneMatch ?? (ifNoneMatch = new HttpEnumHeader(this, Constants.Http.Headers.IfNoneMatch)); }
        }

        public HttpIfRangeHeader IfRange
        {
            get { return ifRange ?? (ifRange = new HttpIfRangeHeader(this)); }
        }

        public HttpDateHeader IfUnmodifiedSince
        {
            get { return ifUnmodifiedSince ?? (ifUnmodifiedSince = new HttpDateHeader(this, Constants.Http.Headers.IfUnmodifiedSince)); }
        }

        public HttpLongHeader MaxForwards
        {
            get { return maxForwards ?? (maxForwards = new HttpLongHeader(this, Constants.Http.Headers.MaxForwards)); }
        }

        public HttpStringHeader Pragma
        {
            get { return pragma ?? (pragma = new HttpStringHeader(this, Constants.Http.Headers.Pragma)); }
        }

        public HttpStringHeader ProxyAuthorization
        {
            get { return proxyAuthorization ?? (proxyAuthorization = new HttpStringHeader(this, Constants.Http.Headers.ProxyAuthorization)); }
        }

        public HttpRangeHeader Range
        {
            get { return range ?? (range = new HttpRangeHeader(this)); }
        }

        public HttpStringHeader Referer
        {
            get { return referer ?? (referer = new HttpStringHeader(this, Constants.Http.Headers.Referer)); }
        }

        public HttpEnumHeader Te
        {
            get { return te ?? (te = new HttpEnumHeader(this, Constants.Http.Headers.TE)); }
        }

        public HttpStringHeader UserAgent
        {
            get { return userAgent ?? (userAgent = new HttpStringHeader(this, Constants.Http.Headers.UserAgent)); }
        }
    }
}
