using Ormikon.Owin.Static.Wrappers;
using System.IO;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.ResponseSender
{
    interface IResponseSender
    {
        Task SendAsync(IResponseHeaders headers, Stream responseStream, IOwinResponse owinResponse);
    }
}
