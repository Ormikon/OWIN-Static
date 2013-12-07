using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ormikon.Owin.Static.Extensions
{
    internal static class HttpHeaderExtensions
    {
        public static void CopyTo(this IDictionary<string, string[]> headers, IDictionary<string, string[]> destination)
        {
            foreach (var header in headers)
            {
                destination[header.Key] = header.Value;
            }
        }

        public static string GetSingleValue(this IDictionary<string, string[]> headers, string header)
        {
            string[] headerValues;
            if (headers.TryGetValue(header, out headerValues) && headerValues.Length > 0)
            {
                return headerValues[0];
            }
            return null;
        }

        public static void SetSingleValue(this IDictionary<string, string[]> headers, string header, string value)
        {
            string[] headerValues;
            if (headers.TryGetValue(header, out headerValues))
            {
                if (value == null)
                    headers.Remove(header);
                else if (headerValues.Length > 0)
                    headerValues[0] = value;
                else
                    headers[header] = new[] {value};
            }
            else if (value != null)
            {
                headers[header] = new[] {value};
            }
        }

        private static string ToRfc1123DateString(this DateTimeOffset? dateTimeOffset)
        {
            return dateTimeOffset.HasValue ? dateTimeOffset.Value.ToString("R") : null;
        }

        private static DateTimeOffset? Rfc1123StringToDateTimeOffset(this string rfc1123)
        {
            if (string.IsNullOrEmpty(rfc1123))
                return null;
            return DateTimeOffset.ParseExact(rfc1123, "R", CultureInfo.InvariantCulture);
        }

        public static DateTimeOffset? GetDateTimeOffset(this IDictionary<string, string[]> headers, string header)
        {
            return headers.GetSingleValue(header).Rfc1123StringToDateTimeOffset();
        }

        public static void SetDateTimeOffset(this IDictionary<string, string[]> headers, string header, DateTimeOffset? value)
        {
            headers.SetSingleValue(header, value.ToRfc1123DateString());
        }

        public static int? GetIntValue(this IDictionary<string, string[]> headers, string header)
        {
            string val = headers.GetSingleValue(header);
            return string.IsNullOrEmpty(val) ? null : new int?(int.Parse(val));
        }

        public static void SetIntValue(this IDictionary<string, string[]> headers, string header, int? value)
        {
            headers.SetSingleValue(header, value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null);
        }

        public static long? GetLongValue(this IDictionary<string, string[]> headers, string header)
        {
            string val = headers.GetSingleValue(header);
            return string.IsNullOrEmpty(val) ? null : new long?(long.Parse(val));
        }

        public static void SetLongValue(this IDictionary<string, string[]> headers, string header, long? value)
        {
            headers.SetSingleValue(header, value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null);
        }
    }
}
