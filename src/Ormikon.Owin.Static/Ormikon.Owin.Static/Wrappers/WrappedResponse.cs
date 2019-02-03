using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.Wrappers
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

        public Stream Body
        {
            get { return response.Body; }
        }

        public IHttpResponseHeaders Headers
        {
            get { return responseHeaders ?? (responseHeaders = new HttpResponseHeaders(response.Headers)); }
        }

        public int StatusCode
        {
            get { return response.StatusCode; }
            set { response.StatusCode = value; }
        }

        public string ReasonPhrase
        {
            get { return responseFeature?.ReasonPhrase; }
            set { if (responseFeature != null) responseFeature.ReasonPhrase = value; }
        }
    }
}
