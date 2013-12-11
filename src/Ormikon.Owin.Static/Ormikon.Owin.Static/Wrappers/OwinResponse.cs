using System.Collections.Generic;
using System.IO;
using Ormikon.Owin.Static.Extensions;
using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.Wrappers
{
    internal class OwinResponse : IOwinResponse
    {
        private readonly IDictionary<string, object> data;
        private readonly IHttpResponseHeaders responseHeaders;

        public OwinResponse(IDictionary<string, object> data)
        {
            this.data = data;
            responseHeaders = new HttpResponseHeaders(data.Get<IDictionary<string, string[]>>(Constants.Owin.Response.Headers));
        }

        public Stream Body
        {
            get { return data.Get<Stream>(Constants.Owin.Response.Body); }
        }

        public IHttpResponseHeaders Headers
        {
            get { return responseHeaders; }
        }

        public int StatusCode
        {
            get { return data.Get<int>(Constants.Owin.Response.StatusCode); }
            set { data[Constants.Owin.Response.StatusCode] = value; }
        }

        public string ReasonPhrase
        {
            get { return data.Get<string>(Constants.Owin.Response.ReasonPhrase); }
            set { data[Constants.Owin.Response.ReasonPhrase] = value; }
        }

        public string Protocol
        {
            get { return data.Get<string>(Constants.Owin.Response.Protocol); }
            set { data[Constants.Owin.Response.Protocol] = value; }
        }
    }
}
