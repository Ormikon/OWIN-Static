using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ormikon.Owin.Static.Headers
{
    internal class HttpLongHeader : HttpHeader
    {
        public HttpLongHeader(IDictionary<string, string[]> headers, string code)
            : base(headers, code)
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

        public long? Value
        {
            get { return GetLongValue(); }
            set { SetLongValue(value); }
        }
    }
}

