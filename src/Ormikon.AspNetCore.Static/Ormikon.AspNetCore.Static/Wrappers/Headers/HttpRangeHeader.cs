using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpRangeHeader : HttpStringHeader
    {
        public HttpRangeHeader(IDictionary<string, StringValues> headers)
            : base(headers, Constants.Http.Headers.Range)
        {
        }

        protected HttpRange GetRange()
        {
            string rangeStr = GetSingleValue();
            return string.IsNullOrEmpty(rangeStr) ? null : new HttpRange(rangeStr);
        }

        protected void SetRange(HttpRange range)
        {
            SetSingleValue(range?.ToString());
        }

        public HttpRange Range
        {
            get => GetRange();
            set => SetRange(value);
        }
    }

    internal class HttpRange
    {
        private readonly long? start;
        private readonly long? end;

        public HttpRange(long range)
        {
            Valid = range != 0;
            if (range < 0)
            {
                start = null;
                end = range;
            }
            else
            {
                start = range;
                end = null;
            }
        }

        public HttpRange(long start, long end)
        {
            this.start = start;
            this.end = end;
            Valid = start >= end;
        }

        public HttpRange(string rangeValue)
        {
            Valid = ParseRangeValue(rangeValue, out start, out end);
        }

        private static Tuple<long?, long?> ParseRangePart(string rangePart)
        {
            if (rangePart.Length == 0)
                return null;
            int rangeSeparatorPos = rangePart.IndexOf('-');
            if (rangeSeparatorPos < 0)
                return null;
            if (rangeSeparatorPos == 0)
            {
                if (!long.TryParse(rangePart.Substring(1).TrimStart(), out var rangeEnd))
                    return null;
                return new Tuple<long?, long?>(null, -rangeEnd);
            }
            if (rangeSeparatorPos == rangePart.Length - 1)
            {
                if (!long.TryParse(rangePart.Remove(rangePart.Length - 1).TrimEnd(), out var rangeStart))
                    return null;
                return new Tuple<long?,long?>(rangeStart, null);
            }

            if (!long.TryParse(rangePart.Remove(rangeSeparatorPos).TrimEnd(), out var start))
                return null;
            if (!long.TryParse(rangePart.Substring(rangeSeparatorPos + 1).TrimStart(), out var end))
                return null;
            return start > end ? null : new Tuple<long?,long?>(start, end);
        }

        private static bool ParseRangeValue(string rangeValue, out long? start, out long? end)
        {
            start = null;
            end = null;
            if (string.IsNullOrEmpty(rangeValue))
                return false;
            var ranges = rangeValue.Split(',');
            for (int i = 0; i < ranges.Length; i++)
            {
                var parsed = ParseRangePart(ranges[i].Trim());
                if (parsed == null)
                    return false;
                if (i == 0)
                {
                    start = parsed.Item1;
                    end = parsed.Item2;
                }
                else if (!start.HasValue || !end.HasValue || !parsed.Item1.HasValue || !parsed.Item2.HasValue)
                    return false;
                else if (end.Value + 1 != parsed.Item1.Value)
                    return false;
                else
                {
                    end = parsed.Item2;
                }
            }
            return true;
        }

        public override string ToString()
        {
            if (!start.HasValue && !end.HasValue)
                return "";
            if (!start.HasValue)
                return end.Value.ToString(CultureInfo.InvariantCulture);
            if (!end.HasValue)
                return start.Value.ToString(CultureInfo.InvariantCulture) + "-";
            return start.Value.ToString(CultureInfo.InvariantCulture) + "-"
                + end.Value.ToString(CultureInfo.InvariantCulture);
        }

        public long? Start => start;

        public long? End => end;

        public bool Valid { get; }
    }
}
