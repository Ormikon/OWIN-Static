using System.Collections.Generic;
using System.IO;
using Ormikon.Owin.Static.Extensions;
using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.Wrappers
{
    internal class OwinRequest : IOwinRequest
    {
        private readonly IDictionary<string, object> data;
        private readonly IHttpRequestHeaders requestHeaders;

        public OwinRequest(IDictionary<string, object> data)
        {
            this.data = data;
            requestHeaders = new HttpRequestHeaders(data.Get<IDictionary<string, string[]>>(Constants.Owin.Request.Headers));
        }

        public Stream Body
        {
            get { return data.Get<Stream>(Constants.Owin.Request.Body); }
        }

        public IHttpRequestHeaders Headers
        {
            get { return requestHeaders; }
        }

        public string Method
        {
            get { return data.Get<string>(Constants.Owin.Request.Method); }
        }

        public string Path
        {
            get { return data.Get<string>(Constants.Owin.Request.Path); }
            set { data[Constants.Owin.Request.Path] = value; }
        }

        public string PathBase
        {
            get { return data.Get<string>(Constants.Owin.Request.PathBase); }
            set { data[Constants.Owin.Request.PathBase] = value; }
        }

        public Location Location
        {
            get { return new Location(Path, PathBase); }
        }

        public string Protocol
        {
            get { return data.Get<string>(Constants.Owin.Request.Protocol); }
        }

        public string QueryString
        {
            get { return data.Get<string>(Constants.Owin.Request.QueryString); }
        }

        public string Scheme
        {
            get { return data.Get<string>(Constants.Owin.Request.Scheme); }
        }
    }
}
