using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpIfRangeHeader : HttpDateHeader
    {
        public HttpIfRangeHeader(IDictionary<string, StringValues> headers)
            : base(headers, Constants.Http.Headers.IfRange)
        {
        }

        public bool IsDate
        {
            get { return Value.HasValue; }
        }

        public string Entity
        {
            get { return GetSingleValue(); }
            set { SetSingleValue(value); }
        }
    }
}
