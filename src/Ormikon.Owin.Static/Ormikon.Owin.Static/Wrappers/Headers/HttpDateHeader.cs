using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpDateHeader : HttpHeader
    {
        public HttpDateHeader(IDictionary<string, StringValues> headers, string code)
            : base(headers, code)
        {
        }

        protected DateTimeOffset? GetRfc1123DateValue()
        {
            var stringValue = GetSingleValue();
            return string.IsNullOrEmpty(stringValue) ? null
                    : new DateTimeOffset?(DateTimeOffset.ParseExact(stringValue, "R", CultureInfo.InvariantCulture));
        }

        protected void SetRfc1123DateValue(DateTimeOffset? value)
        {
            SetSingleValue(value.HasValue ? value.Value.ToString("R") : null);
        }

        public DateTimeOffset? Value
        {
            get { return GetRfc1123DateValue(); }
            set { SetRfc1123DateValue(value); }
        }
    }
}

