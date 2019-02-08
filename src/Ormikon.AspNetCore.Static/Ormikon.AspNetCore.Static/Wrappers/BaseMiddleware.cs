using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ormikon.AspNetCore.Static.Wrappers
{
    internal abstract class BaseMiddleware
    {
        private readonly RequestDelegate next;

        protected BaseMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var wrappedContext = new WrappedContext(context);
            await InvokeAsync(wrappedContext, context.RequestAborted);
        }

        protected virtual async Task InvokeAsync(IWrappedContext context, CancellationToken cancellationToken)
        {
            await Next(context);
        }

        protected async Task Next(IWrappedContext context)
        {
            await next(((WrappedContext)context).CoreContext);
        }
    }
}
