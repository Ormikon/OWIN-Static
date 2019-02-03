using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Ormikon.Owin.Static.Wrappers
{
    internal class WrappedContext : IWrappedContext
    {
        private readonly HttpContext coreContext;
        private IWrappedRequest request;
        private IWrappedResponse response;

        public WrappedContext(HttpContext coreContext)
        {
            this.coreContext = coreContext;
        }

        #region IOwinContext

        public CancellationToken CallCancelled
        {
            get { return coreContext.RequestAborted; }
        }

        public IWrappedRequest Request
        {
            get { return request ?? (request = new WrappedRequest(coreContext.Request)); }
        }

        public IWrappedResponse Response
        {
            get { return response ?? (response = new WrappedResponse(coreContext.Response)); }
        }

        public HttpContext CoreContext
        {
            get { return coreContext; }
        }

        #endregion
    }
}
