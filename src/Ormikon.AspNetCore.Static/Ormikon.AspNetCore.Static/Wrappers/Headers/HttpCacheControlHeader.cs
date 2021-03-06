using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpCacheControlHeader : HttpEnumHeader
    {
        public HttpCacheControlHeader(IDictionary<string, StringValues> headers)
            : base(headers, Constants.Http.Headers.CacheControl)
        {
        }

        protected int? GetMaxAge()
        {
            foreach(var val in GetEnumValues())
            {
                if (val.IndexOf("max-age", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var splits = val.Split('=');
                    if (splits.Length == 2 && splits[0].Trim().ToLowerInvariant() == "max-age")
                    {
                        if (int.TryParse(splits[1], out var maxAge))
                            return maxAge;
                    }
                }
            }

            return null;
        }

        protected void SetMaxAge(int? maxAge)
        {
            var enumValues = new List<string>(GetEnumValues());
            bool found = false;
            for (int i = 0; i < enumValues.Count; i++)
            {
                string val = enumValues[i];
                if (val.IndexOf("max-age", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    found = true;
                    if (maxAge.HasValue)
                    {
                        enumValues[i] = "max-age=" + maxAge.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        enumValues.RemoveAt(i);
                    }
                    break;
                }
            }
            if (!found && maxAge.HasValue)
            {
                enumValues.Add("max-age=" + maxAge.Value.ToString(CultureInfo.InvariantCulture));
                SetEnumValues(enumValues);
            }
            else if (found && !maxAge.HasValue)
            {
                SetEnumValues(enumValues);
            }
        }

        public int? MaxAge
        {
            get => GetMaxAge();
            set => SetMaxAge(value);
        }
    }
}

