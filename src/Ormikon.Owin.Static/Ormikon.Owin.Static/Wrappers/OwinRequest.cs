using System.Collections.Generic;
using System.IO;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static.Wrappers
{
    internal class OwinRequest : IOwinRequest
    {
        private readonly IDictionary<string, object> data;

        public OwinRequest(IDictionary<string, object> data)
        {
            this.data = data;
        }

        public Stream Body
        {
            get { return data.Get<Stream>(Constants.Owin.Request.Body); }
        }

        public IDictionary<string, string[]> Headers
        {
            get { return data.Get<IDictionary<string, string[]>>(Constants.Owin.Request.Headers); }
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
