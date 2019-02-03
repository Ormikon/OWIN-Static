using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ormikon.Owin.Static.Wrappers
{
    internal abstract class BaseMiddleware
    {
        private readonly RequestDelegate next;

        protected BaseMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            try
            {
                var wrappedContext = new WrappedContext(context);
                return Invoke(wrappedContext);
            }
            catch(Exception exception)
            {
                var tcs = new TaskCompletionSource<object>();
                tcs.SetException(exception);
                return tcs.Task;
            }
        }

        protected virtual Task Invoke(IWrappedContext context)
        {
            return Next(context);
        }

        protected Task Next(IWrappedContext context)
        {
            return next(context.CoreContext);
        }
    }
}
