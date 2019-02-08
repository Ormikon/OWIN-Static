using Microsoft.AspNetCore.Http;

namespace Ormikon.AspNetCore.Static.Wrappers
{
    internal class WrappedContext : IWrappedContext
    {
        private IWrappedRequest request;
        private IWrappedResponse response;

        public WrappedContext(HttpContext coreContext)
        {
            CoreContext = coreContext;
        }

        #region IOwinContext

        public IWrappedRequest Request => request ?? (request = new WrappedRequest(CoreContext.Request));

        public IWrappedResponse Response => response ?? (response = new WrappedResponse(CoreContext.Response));

        public HttpContext CoreContext { get; }

        #endregion
    }
}
