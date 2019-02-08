using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Ormikon.AspNetCore.Static.Wrappers
{
    internal interface IWrappedContext
    {
        CancellationToken CallCancelled { get; }

        IWrappedRequest Request { get; }

        IWrappedResponse Response { get; }

        HttpContext CoreContext { get; }
    }
}
