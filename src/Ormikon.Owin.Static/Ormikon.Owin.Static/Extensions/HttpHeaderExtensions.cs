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

        private static string ToRfc1123DateString(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("R") : null;
        }

        private static string ToRfc1123DateString(this DateTimeOffset? dateTimeOffset)
        {
            return dateTimeOffset.HasValue ? dateTimeOffset.Value.ToString("R") : null;
        }

        private static DateTime? Rfc1123StringToDateTime(this string rfc1123)
        {
            if (string.IsNullOrEmpty(rfc1123))
                return null;
            DateTime dt;
            if (DateTime.TryParseExact(rfc1123, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            return null;
        }

        private static DateTimeOffset? Rfc1123StringToDateTimeOffset(this string rfc1123)
        {
            if (string.IsNullOrEmpty(rfc1123))
                return null;
            DateTimeOffset dto;
            if (DateTimeOffset.TryParseExact(rfc1123, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out dto))
                return dto;
            return null;
        }

        public static DateTime? GetDateTime(this IDictionary<string, string[]> headers, string header)
        {
            return headers.GetSingleValue(header).Rfc1123StringToDateTime();
        }

        public static void SetDateTime(this IDictionary<string, string[]> headers, string header, DateTime? value)
        {
            headers.SetSingleValue(header, value.ToRfc1123DateString());
        }

        public static DateTimeOffset? GetDateTimeOffset(this IDictionary<string, string[]> headers, string header)
        {
            return headers.GetSingleValue(header).Rfc1123StringToDateTimeOffset();
        }

        public static void SetDateTimeOffset(this IDictionary<string, string[]> headers, string header, DateTimeOffset? value)
        {
            headers.SetSingleValue(header, value.ToRfc1123DateString());
        }
    }
}
