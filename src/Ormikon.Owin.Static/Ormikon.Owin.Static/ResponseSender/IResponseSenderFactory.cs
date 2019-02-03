using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;

namespace Ormikon.Owin.Static.ResponseSender
{
    internal interface IResponseSenderFactory
    {
        IResponseSender CreateSenderFor(IStaticResponse response, IWrappedContext context);
    }
}
