using System;

namespace Ormikon.Owin.Static.Headers
{
    internal interface IHttpHeader
    {
        void Clear();

        string[] Values { get; }
    }
}

