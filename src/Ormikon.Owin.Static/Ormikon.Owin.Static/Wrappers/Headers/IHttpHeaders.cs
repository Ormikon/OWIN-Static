using System;
using System.Collections.Generic;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal interface IHttpHeaders : IDictionary<string, string[]>, ICloneable
    {
        void CopyTo(IDictionary<string, string[]> headers, params string[] except);
        new IHttpHeaders Clone();
    }
}

