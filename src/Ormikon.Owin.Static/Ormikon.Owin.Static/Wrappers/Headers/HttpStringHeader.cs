using System;
using System.Collections.Generic;

namespace Ormikon.Owin.Static.Headers
{
    internal class HttpStringHeader : HttpHeader
    {
        public HttpStringHeader(IDictionary<string, string[]> headers, string code)
            : base(headers, code)
        {
        }

        public string Value
        {
            get { return GetSingleValue(); }
            set { SetSingleValue(value); }
        }
    }
}

