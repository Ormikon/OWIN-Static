using System.IO;
using System.Threading.Tasks;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    interface IResponseSender
    {
        Task SendAsync(IStaticResponse response, Stream responseStream, IWrappedContext context);
    }
}
