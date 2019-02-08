using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpRetryAfterHeader : HttpDateHeader
    {
        public HttpRetryAfterHeader(IDictionary<string, StringValues> headers)
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
            SetSingleValue(value?.ToString(CultureInfo.InvariantCulture));
        }

        public bool IsDate => Value.HasValue;

        public long? Seconds
        {
            get => GetLongValue();
            set => SetLongValue(value);
        }
    }
}
