using System.IO;
using Microsoft.AspNetCore.Http;
using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.Wrappers
{
    internal class WrappedRequest : IWrappedRequest
    {
        private readonly HttpRequest request;
        private IHttpRequestHeaders requestHeaders;

        public WrappedRequest(HttpRequest request)
        {
            this.request = request;
        }

        public Stream Body
        {
            get { return request.Body; }
        }

        public IHttpRequestHeaders Headers
        {
            get { return requestHeaders ?? (requestHeaders = new HttpRequestHeaders(request.Headers)); }
        }

        public string Method
        {
            get { return request.Method; }
        }

        public string Path
        {
            get { return request.Path; }
            set { request.Path = value; }
        }

        public string PathBase
        {
            get { return request.PathBase; }
            set { request.PathBase = value; }
        }

        public Location Location
        {
            get { return new Location(Path, PathBase); }
            set { value.SetToRequest(request); }
        }

        public string Protocol
        {
            get { return request.Protocol; }
        }

        public QueryString QueryString
        {
            get { return request.QueryString; }
        }

        public string Scheme
        {
            get { return request.Scheme; }
        }
    }
}
