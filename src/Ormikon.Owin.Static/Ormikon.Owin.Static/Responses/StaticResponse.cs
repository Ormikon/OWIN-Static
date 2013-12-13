using System;
using System.IO;
using System.Text;
using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.Responses
{
    internal class StaticResponse : IStaticResponse, IDisposable
    {
        private const string PlainTextContentType = "text/plain; charset=UTF-8";

        private static readonly string poweredBy;

        private readonly int statusCode;
        private readonly string reasonPhrase;
        private readonly IHttpResponseHeaders headers;
        private readonly Stream body;

        static StaticResponse()
        {
            poweredBy = "Ormikon.Owin.Static " + typeof(StaticResponse).Assembly.GetName().Version;
        }

        public StaticResponse(int statusCode)
            : this(statusCode, null, null)
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
            : this(statusCode, null, contentType, body)
        {
        }

        public StaticResponse(int statusCode, string reasonPhrase, string contentType, Stream body)
        {
            this.statusCode = statusCode;
            this.reasonPhrase = reasonPhrase;
            headers = new HttpResponseHeaders();
            if (!string.IsNullOrEmpty((contentType)))
                headers.ContentType.Value = contentType;
            headers[Constants.Http.Headers.PoweredBy] = new[] {poweredBy};
            this.body = body ?? Stream.Null;
        }

        private static Stream GetBodyFromString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Stream.Null;
            var message = Encoding.UTF8.GetBytes(text);
            return new MemoryStream(message);
        }

        #region Response Builders

        public static StaticResponse HttpStatus(int statusCode)
        {
            return HttpStatus(statusCode, null);
        }

        public static StaticResponse HttpStatus(int statusCode, string reasonPhrase)
        {
            return new StaticResponse(statusCode, reasonPhrase, null, null);
        }

        public static StaticResponse Redirect(string location)
        {
            var result = new StaticResponse(Constants.Http.StatusCodes.Redirection.Found, "Redirecting to " + location);
            result.Headers.Location.Value = location;
            return result;
        }

        public static StaticResponse MethodNotAllowed(params string[] allowedMethods)
        {
            var result = new StaticResponse(Constants.Http.StatusCodes.ClientError.MethodNotAllowed);
            result.Headers.Allow.EnumValues = allowedMethods;
            return result;
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

        public string ReasonPhrase
        {
            get { return reasonPhrase; }
        }

        public IHttpResponseHeaders Headers
        {
            get { return headers; }
        }

        public Stream Body
        {
            get { return body; }
        }
    }
}
