using System;
using System.Collections.Generic;

namespace Ormikon.Owin.Static.Headers
{
    internal interface IHttpHeaders : IDictionary<string, string[]>, ICloneable
    {
        void CopyTo(IDictionary<string, string[]> headers);
        new IHttpHeaders Clone();
    }
}

