using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static
{
    internal class StaticResponse : IResponseHeaders, IDisposable
    {
        private const string PlainTextContentType = "text/plain; charset=UTF-8";

        private static readonly string poweredBy;

        private readonly int statusCode;
        private readonly IDictionary<string, string[]> headers;
        private readonly Stream body;

        static StaticResponse()
        {
            poweredBy = "Ormikon.Owin.Static " + typeof(StaticResponse).Assembly.GetName().Version;
        }

        public StaticResponse(int statusCode)
            : this(statusCode, null)
        {
        }

        public StaticResponse(string text)
            : this(Constants.Http.StatusCodes.Successful.Ok, text)
        {
        }

        public StaticResponse(int statusCode, string text)
            : this(statusCode, PlainTextContentType, GetBodyFromString(text))
        {
        }

        public StaticResponse(string contentType, Stream body)
            : this(Constants.Http.StatusCodes.Successful.Ok, contentType, body)
        {
        }

        public StaticResponse(int statusCode, string contentType, Stream body)
        {
            this.statusCode = statusCode;
            headers = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            ContentType = contentType;
            headers.SetSingleValue(Constants.Http.Headers.PoweredBy, poweredBy);
            this.body = body;
        }

        private static Stream GetBodyFromString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Stream.Null;
            var message = Encoding.UTF8.GetBytes(text);
            return new MemoryStream(message);
        }

        #region Response Builders

        public static StaticResponse Redirect(string location)
        {
            return new StaticResponse(Constants.Http.StatusCodes.Redirection.Found, "Redirecting to " + location)
            {
                Location = location
            };
        }

        public static StaticResponse MethodNotAllowed(params string[] allowedMethods)
        {
            return new StaticResponse(Constants.Http.StatusCodes.ClientError.MethodNotAllowed)
            {
                Allow = string.Join(",", allowedMethods)
            };
        }

        #endregion

        #region Headers

        private static int? ParseMaxAgeValue(string cacheControl)
        {
            if (string.IsNullOrEmpty(cacheControl))
                return null;
            string[] ccParts = cacheControl.Split(',');
            foreach (var ccPart in ccParts)
            {
                if (ccPart.IndexOf("max-age", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    int eqIndex = ccPart.IndexOf('=');
                    if (eqIndex > 0 && eqIndex != ccPart.Length - 1)
                    {
                        string maxAgeValueStr = ccPart.Substring(eqIndex + 1).Trim();
                        if (!string.IsNullOrEmpty(maxAgeValueStr))
                        {
                            int result;
                            if (int.TryParse(maxAgeValueStr, out result))
                                return result;
                        }
                    }
                }
            }

            return null;
        }

        private static string AddMaxAgeInCacheControl(int? maxAge)
        {
            return maxAge.HasValue && maxAge.Value != 0 ? "max-age=" + maxAge.Value : null;
        }

        #endregion

        public void Dispose()
        {
            if (body != null)
                body.Dispose();
        }

        public int StatusCode
        {
            get { return statusCode; }
        }

        public IDictionary<string, string[]> Headers
        {
            get { return headers; }
        }

        public string Allow
        {
            get { return headers.GetSingleValue(Constants.Http.Headers.Allow); }
            set { headers.SetSingleValue(Constants.Http.Headers.Allow, value); }
        }

        public long? ContentLength
        {
            get { return headers.GetLongValue(Constants.Http.Headers.ContentLength); }
            set { headers.SetLongValue(Constants.Http.Headers.ContentLength, value); }
        }

        public string ContentType
        {
            get { return headers.GetSingleValue(Constants.Http.Headers.ContentType); }
            set { headers.SetSingleValue(Constants.Http.Headers.ContentType, value); }
        }

        public DateTimeOffset? Date
        {
            get { return headers.GetDateTimeOffset(Constants.Http.Headers.Date); }
            set { headers.SetDateTimeOffset(Constants.Http.Headers.Date, value); }
        }

        public string ETag
        {
            get { return headers.GetSingleValue(Constants.Http.Headers.ETag); }
            set { headers.SetSingleValue(Constants.Http.Headers.ETag, value); }
        }

        public DateTimeOffset? LastModified
        {
            get { return headers.GetDateTimeOffset(Constants.Http.Headers.LastModified); }
            set { headers.SetDateTimeOffset(Constants.Http.Headers.LastModified, value); }
        }

        public DateTimeOffset? Expires
        {
            get { return headers.GetDateTimeOffset(Constants.Http.Headers.Expires); }
            set
            {
                headers.SetDateTimeOffset(Constants.Http.Headers.Expires,
                                          value.HasValue
                                              ? (value.Value == DateTimeOffset.MinValue ? null : value)
                                              : null);
            }
        }

        public int? MaxAge
        {
            get { return ParseMaxAgeValue(headers.GetSingleValue(Constants.Http.Headers.CacheControl)); }
            set { headers.SetSingleValue(Constants.Http.Headers.CacheControl, AddMaxAgeInCacheControl(value)); }
        }

        public string Location
        {
            get { return headers.GetSingleValue(Constants.Http.Headers.Location); }
            set { headers.SetSingleValue(Constants.Http.Headers.Location, value); }
        }

        public Stream Body
        {
            get { return body; }
        }
    }
}
