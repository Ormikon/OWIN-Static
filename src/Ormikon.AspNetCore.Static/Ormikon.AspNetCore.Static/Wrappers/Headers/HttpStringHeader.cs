using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpStringHeader : HttpHeader
    {
        public HttpStringHeader(IDictionary<string, StringValues> headers, string code)
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

