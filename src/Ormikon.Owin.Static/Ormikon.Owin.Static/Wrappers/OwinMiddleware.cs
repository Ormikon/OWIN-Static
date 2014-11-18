using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.Wrappers
{
    internal abstract class OwinMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> next;

        protected OwinMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            try
            {
                var context = new OwinContext(environment);
                return Invoke(context);
            }
            catch(Exception exception)
            {
                var tcs = new TaskCompletionSource<object>();
                tcs.SetException(exception);
                return tcs.Task;
            }
        }

        protected virtual Task Invoke(IOwinContext context)
        {
            return Next(context);
        }

        protected Task Next(IOwinContext context)
        {
            return next(context.Environment);
        }
    }
}
