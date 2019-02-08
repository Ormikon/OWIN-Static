using System.IO;
using Microsoft.AspNetCore.Http;
using Ormikon.AspNetCore.Static.Wrappers.Headers;

namespace Ormikon.AspNetCore.Static.Wrappers
{
    internal class WrappedRequest : IWrappedRequest
    {
        private readonly HttpRequest request;
        private IHttpRequestHeaders requestHeaders;

        public WrappedRequest(HttpRequest request)
        {
            this.request = request;
        }

        public Stream Body => request.Body;

        public IHttpRequestHeaders Headers => requestHeaders ?? (requestHeaders = new HttpRequestHeaders(request.Headers));

        public string Method => request.Method;

        public string Path
        {
            get => request.Path;
            set => request.Path = value;
        }

        public string PathBase
        {
            get => request.PathBase;
            set => request.PathBase = value;
        }

        public Location Location
        {
            get => new Location(Path, PathBase);
            set => value.SetToRequest(request);
        }

        public string Protocol => request.Protocol;

        public QueryString QueryString => request.QueryString;

        public string Scheme => request.Scheme;
    }
}
