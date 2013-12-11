using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpContentRangeHeader : HttpHeader
    {
        public HttpContentRangeHeader(IDictionary<string, string[]> headers)
            : base(headers, Constants.Http.Headers.ContentRange)
        {

        }

        protected HttpContentRange GetContentRange()
        {
            string contentRangeStr = GetSingleValue();
            return string.IsNullOrEmpty(contentRangeStr) ? null : new HttpContentRange(contentRangeStr);
        }

        protected void SetContentRange(HttpContentRange range)
        {
            SetSingleValue(range == null ? null : range.ToString());
        }

        public HttpContentRange Range
        {
            get { return GetContentRange(); }
            set { SetContentRange(value); }
        }
    }

    internal class HttpContentRange
    {
        private readonly long start;
        private readonly long end;
        private readonly long length;
        private readonly bool valid;

        public HttpContentRange(string range)
        {
            ParseRange(range, out start, out end, out length);
            valid = CheckRange(start, end, length);
        }

        public HttpContentRange(long start, long end, long length)
        {
            this.start = start;
            this.end = end;
            this.length = length;
            valid = CheckRange(start, end, length);
        }

        private static void ParseRange(string range, out long start, out long end, out long length)
        {
            start = 0;
            end = 0;
            length = 0;
            if (string.IsNullOrEmpty(range))
                return;
            int rangeSeparatorPos = range.IndexOf('-');
            if (rangeSeparatorPos <= 0)
                return;
            int lenSeparatorPos = range.IndexOf('/', rangeSeparatorPos);
            if (lenSeparatorPos < 0)
                return;
            string part = range.Substring(0, rangeSeparatorPos).Trim();
            if (part.Length == 0)
                return;
            if (!long.TryParse(part, out start))
                return;
            part = range.Substring(rangeSeparatorPos + 1, lenSeparatorPos - rangeSeparatorPos - 1).Trim();
            if (part.Length == 0)
                return;
            if (!long.TryParse(part, out end))
                return;
            part = range.Substring(lenSeparatorPos + 1).Trim();
            if (part.Length == 0)
                return;
            if (part == "*")
            {
                length = -1;
            }
            else
                long.TryParse(part, out length);
        }

        private static bool CheckRange(long start, long end, long length)
        {
            if (start < 0 || end < 0)
                return false;
            if (start >= end)
                return false;
            if (length == 0)
                return false;
            if (length > 0 && end >= length)
                return false;
            return true;
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append(start.ToString(CultureInfo.InvariantCulture));
            b.Append('-');
            b.Append(end.ToString(CultureInfo.InvariantCulture));
            b.Append('/');
            if (length < 0)
                b.Append('*');
            else
                b.Append(length.ToString(CultureInfo.InvariantCulture));
            return b.ToString();
        }

        public long Start
        {
            get { return start; }
        }

        public long End
        {
            get { return end; }
        }

        public long Length
        {
            get { return length; }
        }

        public bool Valid
        {
            get { return valid; }
        }
    }
}
