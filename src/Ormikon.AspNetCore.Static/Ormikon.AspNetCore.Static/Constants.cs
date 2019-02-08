namespace Ormikon.AspNetCore.Static
{
    internal static class Constants
    {
        public static class Http
        {
            public static class Methods
            {
                public const string Options = "OPTIONS";
                public const string Get = "GET";
                public const string Post = "POST";
                public const string Head = "HEAD";
                public const string Put = "PUT";
                public const string Delete = "DELETE";
                public const string Trace = "TRACE";
                public const string Connect = "CONNECT";
            }

            public static class Headers
            {
                public const string Accept = "Accept";
                public const string AcceptCharset = "Accept-Charset";
                public const string AcceptEncoding = "Accept-Encoding";
                public const string AcceptLanguage = "Accept-Language";
                public const string AcceptRanges = "Accept-Ranges";
                public const string Age = "Age";
                public const string Allow = "Allow";
                public const string Authorization = "Authorization";
                public const string CacheControl = "Cache-Control";
                public const string Connection = "Connection";
                public const string ContentDisposition = "Content-Disposition";
                public const string ContentEncoding = "Content-Encoding";
                public const string ContentLanguage = "Content-Language";
                public const string ContentLength = "Content-Length";
                public const string ContentLocation = "Content-Location";
                public const string ContentMd5 = "Content-MD5";
                public const string ContentRange = "Content-Range";
                public const string ContentType = "Content-Type";
                public const string Cookie = "Cookie";
                public const string Date = "Date";
                public const string ETag = "ETag";
                public const string Expect = "Expect";
                public const string Expires = "Expires";
                public const string From = "From";
                public const string Host = "Host";
                public const string IfMatch = "If-Match";
                public const string IfModifiedSince = "If-Modified-Since";
                public const string IfNoneMatch = "If-None-Match";
                public const string IfRange = "If-Range";
                public const string IfUnmodifiedSince = "If-Unmodified-Since";
                public const string LastModified = "Last-Modified";
                public const string Link = "Link";
                public const string Location = "Location";
                public const string MaxForwards = "Max-Forwards";
                public const string Pragma = "Pragma";
                public const string ProxyAuthenticate = "Proxy-Authenticate";
                public const string ProxyAuthorization = "Proxy-Authorization";
                public const string Public = "Public";
                public const string Range = "Range";
                public const string Referer = "Referer";
                public const string RetryAfter = "Retry-After";
                public const string Server = "Server";
                public const string SetCookie = "Set-Cookie";
                public const string Title = "Title";
// ReSharper disable InconsistentNaming
                public const string TE = "TE";
// ReSharper restore InconsistentNaming
                public const string Trailer = "Trailer";
                public const string TransferEncoding = "Transfer-Encoding";
                public const string Upgrade = "Upgrade";
                public const string UserAgent = "UserAgent";
                public const string Vary = "Vary";
                public const string Via = "Via";
                public const string Warning = "Warning";
                public const string WwwAuthenticate = "WWW-Authenticate";
                public const string PoweredBy = "X-Powered-By";
            }

            public static class StatusCodes
            {
                public static class Informational
                {
                    public const int Continue = 100;
                    public const int SwitchingProtocols = 101;
                }

                public static class Successful
                {
                    public const int Ok = 200;
                    public const int Created = 201;
                    public const int Accepted = 202;
                    public const int NonAuthoritativeInformation = 203;
                    public const int NoContent = 204;
                    public const int ResetContent = 205;
                    public const int PartialContent = 206;
                }

                public static class Redirection
                {
                    public const int MultipleChoices = 300;
                    public const int MovedPermanently = 301;
                    public const int Found = 302;
                    public const int SeeOther = 303;
                    public const int NotModified = 304;
                    public const int UseProxy = 305;
                    public const int TemporaryRedirect = 307;
                }

                public static class ClientError
                {
                    public const int BadRequest = 400;
                    public const int Unauthorized = 401;
                    public const int PaymentRequired = 402;
                    public const int Forbidden = 403;
                    public const int NotFound = 404;
                    public const int MethodNotAllowed = 405;
                    public const int NotAcceptable = 406;
                    public const int ProxyAuthenticationRequired = 407;
                    public const int RequestTimeout = 408;
                    public const int Conflict = 409;
                    public const int Gone = 410;
                    public const int LengthRequired = 411;
                    public const int PreconditionFailed = 412;
                    public const int RequestEntityTooLarge = 413;
                    public const int RequestUriTooLong = 414;
                    public const int UnsupportedMediaType = 415;
                    public const int RequestedRangeNotSatisfiable = 416;
                    public const int ExpectationFailed = 417;
                }

                public static class Error
                {
                    public const int InternalServerError = 500;
                    public const int NotImplemented = 501;
                    public const int BadGateway = 502;
                    public const int ServiceUnavailable = 503;
                    public const int GatewayTimeout = 504;
                    public const int HttpVersionNotSupported = 505;
                }
            }
        }

        public static class Owin
        {
            #region Owin itself

            private const string OwinPrefix = "owin.";
            public const string Version = OwinPrefix + "Version";
            public const string CallCancelled = OwinPrefix + "CallCancelled";

            public static class Request
            {
                private const string Prefix = OwinPrefix + "Request";
                public const string Body = Prefix + "Body";
                public const string Headers = Prefix + "Headers";
                public const string Method = Prefix + "Method";
                public const string Path = Prefix + "Path";
                public const string PathBase = Prefix + "PathBase";
                public const string Protocol = Prefix + "Protocol";
                public const string QueryString = Prefix + "QueryString";
                public const string Scheme = Prefix + "Scheme";
            }

            public static class Response
            {
                private const string Prefix = OwinPrefix + "Response";
                public const string Body = Prefix + "Body";
                public const string Headers = Prefix + "Headers";
                public const string StatusCode = Prefix + "StatusCode";
                public const string ReasonPhrase = Prefix + "ReasonPhrase";
                public const string Protocol = Prefix + "Protocol";
            }

            #endregion

            public static class Common
            {
                public static class Ssl
                {
                    private const string Prefix = "ssl.";
                    public const string ClientCertificate = Prefix + "ClientCertificate";
                }

                public static class Server
                {
                    private const string Prefix = "server.";
                    public const string RemoteIpAddress = Prefix + "RemoteIpAddress";
                    public const string RemotePort = Prefix + "RemotePort";
                    public const string LocalIpAddress = Prefix + "LocalIpAddress";
                    public const string LocalPort = Prefix + "LocalPort";
                    public const string IsLocal = Prefix + "IsLocal";
                    public const string Capabilities = Prefix + "Capabilities";
                    public const string OnSendingHeaders = Prefix + "OnSendingHeaders";
                }

                public static class Host
                {
                    private const string Prefix = "host.";
                    public const string TraceOutput = Prefix + "TraceOutput";
                    public const string Addresses = Prefix + "Addresses";
                }
            }

            public static class SendFile
            {
                private const string Prefix = "sendfile.";
// ReSharper disable MemberHidesStaticFromOuterClass
                public const string Version = Prefix + "Version";
// ReSharper restore MemberHidesStaticFromOuterClass
                public const string Support = Prefix + "Support";
                public const string Concurrency = Prefix + "Concurrency";
                public const string SendAsync = Prefix + "SendAsync";
            }

            public static class WebSocket
            {
                private const string Prefix = "websocket.";
// ReSharper disable MemberHidesStaticFromOuterClass
                public const string Version = Prefix + "Version";
// ReSharper restore MemberHidesStaticFromOuterClass
                public const string Accept = Prefix + "Accept";
                public const string SendAsync = Prefix + "SendAsync";
                public const string ReceiveAsync = Prefix + "ReceiveAsync";
                public const string CloseAsync = Prefix + "CloseAsync";
// ReSharper disable MemberHidesStaticFromOuterClass
                public const string CallCancelled = Prefix + "CallCancelled";
// ReSharper restore MemberHidesStaticFromOuterClass
                public const string ClientCloseStatus = Prefix + "ClientCloseStatus";
                public const string ClientCloseDescription = Prefix + "ClientCloseDescription";
            }

            public static class Opaque
            {
                private const string Prefix = "opaque.";
// ReSharper disable MemberHidesStaticFromOuterClass
                public const string Version = Prefix + "Version";
// ReSharper restore MemberHidesStaticFromOuterClass
                public const string Upgrade = Prefix + "Upgrade";
                public const string Stream = Prefix + "Stream";
// ReSharper disable MemberHidesStaticFromOuterClass
                public const string CallCancelled = Prefix + "CallCancelled";
// ReSharper restore MemberHidesStaticFromOuterClass
            }
        }
    }
}
