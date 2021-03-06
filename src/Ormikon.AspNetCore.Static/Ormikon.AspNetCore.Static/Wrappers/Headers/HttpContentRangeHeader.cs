﻿using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpContentRangeHeader : HttpHeader
    {
        public HttpContentRangeHeader(IDictionary<string, StringValues> headers)
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
            SetSingleValue(range?.ToString());
        }

        public HttpContentRange Range
        {
            get => GetContentRange();
            set => SetContentRange(value);
        }
    }

    internal class HttpContentRange
    {
        private readonly long start;
        private readonly long end;
        private readonly long length;

        public HttpContentRange(string range)
        {
            ParseRange(range, out start, out end, out length);
            Valid = CheckRange(start, end, length);
        }

        public HttpContentRange(long start, long end, long length)
        {
            this.start = start;
            this.end = end;
            this.length = length;
            Valid = CheckRange(start, end, length);
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

        public long Start => start;

        public long End => end;

        public long Length => length;

        public bool Valid { get; }
    }
}
