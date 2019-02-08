using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal interface IHttpHeader
    {
        void Clear();

        bool Available { get; }

        StringValues Values { get; }
    }
}

