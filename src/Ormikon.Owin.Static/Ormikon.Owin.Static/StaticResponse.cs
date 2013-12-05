using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static
{
    internal class StaticResponse : IResponseHeaders, IDisposable
    {
        private readonly int status;
        private readonly IDictionary<string, string[]> headers;
        private readonly Stream body;

        public StaticResponse(string location)
            : this(Constants.Http.StatusCodes.Redirection.Found, "text/plain", DateTimeOffset.MinValue,
            0, location, GetRedirectBody(location))
        {
        }

        public StaticResponse(string contentType, Stream body)
            : this(contentType, DateTimeOffset.MinValue, 0, body)
        {
        }

        public StaticResponse(string contentType, DateTimeOffset expires, int maxAge, Stream body)
            : this(Constants.Http.StatusCodes.Successful.Ok, contentType, expires, maxAge, null, body)
        {
        }

        public StaticResponse(int status, string contentType, DateTimeOffset expires, int maxAge, string location, Stream body)
        {
            this.status = status;
            headers = new Dictionary<string, string[]>();
            ContentType = contentType;
            Expires = expires;
            MaxAge = maxAge;
            Location = location;
            this.body = body;
        }

        private static Stream GetRedirectBody(string location)
        {
            var message = Encoding.UTF8.GetBytes("Redirecting to " + location);
            return new MemoryStream(message);
        }

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
            return maxAge.HasValue && maxAge.Value != 0 ? "public, max-age=" + maxAge.Value : null;
        }

        #endregion

        public void Dispose()
        {
            if (body != null)
                body.Dispose();
        }

        public int Status
        {
            get { return status; }
        }

        public IDictionary<string, string[]> Headers
        {
            get { return headers; }
        }

        public string ContentType
        {
            get { return headers.GetSingleValue(Constants.Http.Headers.ContentType); }
            set { headers.SetSingleValue(Constants.Http.Headers.ContentType, value); }
        }

        public DateTime? Date
        {
            get { return headers.GetDateTime(Constants.Http.Headers.Date); }
            set { headers.SetDateTime(Constants.Http.Headers.Date, value); }
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
