using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpRangeHeader : HttpStringHeader
    {
        public HttpRangeHeader(IDictionary<string, string[]> headers)
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
            SetSingleValue(range == null ? null : range.ToString());
        }

        public HttpRange Range
        {
            get { return GetRange(); }
            set { SetRange(value); }
        }
    }

    internal class HttpRange
    {
        private readonly long? start;
        private readonly long? end;
        private readonly bool valid;

        public HttpRange(long range)
        {
            valid = range != 0;
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
            valid = start >= end;
        }

        public HttpRange(string rangeValue)
        {
            valid = ParseRangeValue(rangeValue, out start, out end);
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
                long rangeEnd;
                if (!long.TryParse(rangePart.Substring(1).TrimStart(), out rangeEnd))
                    return null;
                return new Tuple<long?, long?>(null, -rangeEnd);
            }
            if (rangeSeparatorPos == rangePart.Length - 1)
            {
                long rangeStart;
                if (!long.TryParse(rangePart.Remove(rangePart.Length - 1).TrimEnd(), out rangeStart))
                    return null;
                return new Tuple<long?,long?>(rangeStart, null);
            }
            long start;
            if (!long.TryParse(rangePart.Remove(rangeSeparatorPos).TrimEnd(), out start))
                return null;
            long end;
            if (!long.TryParse(rangePart.Substring(rangeSeparatorPos + 1).TrimStart(), out end))
                return null;
            if (start > end)
                return null;
            return new Tuple<long?,long?>(start, end);
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

        public long? Start
        {
            get { return start; }
        }

        public long? End
        {
            get { return end; }
        }

        public bool Valid
        {
            get { return valid; }
        }
    }
}
