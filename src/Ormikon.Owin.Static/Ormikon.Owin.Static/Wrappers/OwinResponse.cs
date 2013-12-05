using System.Collections.Generic;
using System.IO;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static.Wrappers
{
    internal class OwinResponse : IOwinResponse
    {
        private readonly IDictionary<string, object> data;

        public OwinResponse(IDictionary<string, object> data)
        {
            this.data = data;
        }

        public Stream Body
        {
            get { return data.Get<Stream>(Constants.Owin.Response.Body); }
        }

        public IDictionary<string, string[]> Headers
        {
            get { return data.Get<IDictionary<string, string[]>>(Constants.Owin.Response.Headers); }
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
