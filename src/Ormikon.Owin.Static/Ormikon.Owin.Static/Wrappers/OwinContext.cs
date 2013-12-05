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

        public OwinContext(IDictionary<string, object> data)
        {
            this.data = data;
            request = new OwinRequest(data);
            response = new OwinResponse(data);
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

        public IDictionary<string, object> Environment
        {
            get { return data; }
        }

        #endregion
    }
}