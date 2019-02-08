using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpIfRangeHeader : HttpDateHeader
    {
        public HttpIfRangeHeader(IDictionary<string, StringValues> headers)
            : base(headers, Constants.Http.Headers.IfRange)
        {
        }

        public bool IsDate => Value.HasValue;

        public string Entity
        {
            get => GetSingleValue();
            set => SetSingleValue(value);
        }
    }
}
