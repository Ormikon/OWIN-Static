using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal interface IHttpHeaders : IDictionary<string, StringValues>, ICloneable
    {
        void CopyTo(IDictionary<string, StringValues> headers, params string[] except);
        new IHttpHeaders Clone();
    }
}

