using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal interface IResponseSenderFactory
    {
        IResponseSender CreateSenderFor(IStaticResponse response, IWrappedContext context);
    }
}
