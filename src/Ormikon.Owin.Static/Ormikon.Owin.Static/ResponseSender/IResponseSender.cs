using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;
using System.IO;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.ResponseSender
{
    interface IResponseSender
    {
        Task SendAsync(IStaticResponse response, Stream responseStream, IWrappedContext context);
    }
}
