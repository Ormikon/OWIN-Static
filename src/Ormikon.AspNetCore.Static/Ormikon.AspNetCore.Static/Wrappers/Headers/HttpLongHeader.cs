using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpLongHeader : HttpHeader
    {
        public HttpLongHeader(IDictionary<string, StringValues> headers, string code)
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
            SetSingleValue(value?.ToString(CultureInfo.InvariantCulture));
        }

        public long? Value
        {
            get => GetLongValue();
            set => SetLongValue(value);
        }
    }
}

