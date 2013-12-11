using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpRetryAfterHeader : HttpDateHeader
    {
        public HttpRetryAfterHeader(IDictionary<string, string[]> headers)
            : base(headers, Constants.Http.Headers.RetryAfter)
        {
        }

        protected long? GetLongValue()
        {
            string stringValue = GetSingleValue();
            return string.IsNullOrEmpty(stringValue) ? null : new long?(long.Parse(stringValue, CultureInfo.InvariantCulture));
        }

        protected void SetLongValue(long? value)
        {
            SetSingleValue(value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null);
        }

        public bool IsDate
        {
            get { return Value.HasValue; }
        }

        public long? Seconds
        {
            get { return GetLongValue(); }
            set { SetLongValue(value); }
        }
    }
}
