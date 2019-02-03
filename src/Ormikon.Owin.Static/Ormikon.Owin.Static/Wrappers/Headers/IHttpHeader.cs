using Microsoft.Extensions.Primitives;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal interface IHttpHeader
    {
        void Clear();

        bool Available { get; }

        StringValues Values { get; }
    }
}

