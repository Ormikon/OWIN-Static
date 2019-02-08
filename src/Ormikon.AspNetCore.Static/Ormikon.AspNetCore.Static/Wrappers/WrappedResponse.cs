using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Ormikon.AspNetCore.Static.Wrappers.Headers;

namespace Ormikon.AspNetCore.Static.Wrappers
{
    internal class WrappedResponse : IWrappedResponse
    {
        private readonly HttpResponse response;
        private readonly IHttpResponseFeature responseFeature;
        private IHttpResponseHeaders responseHeaders;

        public WrappedResponse(HttpResponse response)
        {
            this.response = response;
            responseFeature = response.HttpContext.Features.Get<IHttpResponseFeature>();
        }

        public Stream Body => response.Body;

        public IHttpResponseHeaders Headers => responseHeaders ?? (responseHeaders = new HttpResponseHeaders(response.Headers));

        public int StatusCode
        {
            get => response.StatusCode;
            set => response.StatusCode = value;
        }

        public string ReasonPhrase
        {
            get => responseFeature?.ReasonPhrase;
            set { if (responseFeature != null) responseFeature.ReasonPhrase = value; }
        }
    }
}
