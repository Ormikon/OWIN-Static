using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpIfRangeHeader : HttpDateHeader
    {
        public HttpIfRangeHeader(IDictionary<string, string[]> headers)
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
