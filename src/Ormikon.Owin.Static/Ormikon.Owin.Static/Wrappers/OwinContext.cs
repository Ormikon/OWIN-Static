using System.Collections.Generic;
using System.Threading;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static.Wrappers
{
    internal class OwinContext : IOwinContext
    {
        private readonly IDictionary<string, object> data;
        private readonly IOwinRequest request;
        private readonly IOwinResponse response;
        private readonly IOwinSsl ssl;
        private readonly IOwinHost host;
        private readonly IOwinServer server;

        public OwinContext(IDictionary<string, object> data)
        {
            this.data = data;
            request = new OwinRequest(data);
            response = new OwinResponse(data);
            ssl = new OwinSsl(data);
            host = new OwinHost(data);
            server = new OwinServer(data);
        }

        #region IOwinContext

        public string Version
        {
            get { return data.Get<string>(Constants.Owin.Version); }
        }

        public CancellationToken CallCancelled
        {
            get { return data.Get<CancellationToken>(Constants.Owin.CallCancelled); }
        }

        public IOwinRequest Request
        {
            get { return request; }
        }

        public IOwinResponse Response
        {
            get { return response; }
        }

        public IOwinSsl Ssl
        {
            get { return ssl; }
        }

        public IOwinHost Host
        {
            get { return host; }
        }

        public IOwinServer Server
        {
            get { return server; }
        }

        public IDictionary<string, object> Environment
        {
            get { return data; }
        }

        #endregion
    }
}