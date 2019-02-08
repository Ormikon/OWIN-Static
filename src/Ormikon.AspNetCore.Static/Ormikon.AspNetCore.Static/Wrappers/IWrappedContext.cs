using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Ormikon.AspNetCore.Static.Wrappers
{
    internal interface IWrappedContext
    {
        IWrappedRequest Request { get; }

        IWrappedResponse Response { get; }
    }
}
