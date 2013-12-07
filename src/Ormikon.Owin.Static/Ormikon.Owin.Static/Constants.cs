using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static
{
    internal static class Constants
    {
        public class Http
        {
            public class Methods
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

            public class Headers
            {
                public const string AcceptEncoding = "Accept-Encoding";
                public const string AcceptRanges = "Accept-Ranges";
                public const string Age = "Age";
                public const string Allow = "Allow";
                public const string CacheControl = "Cache-Control";
                public const string ContentEncoding = "Content-Encoding";
                public const string ContentLength = "Content-Length";
                public const string ContentRange = "Content-Range";
                public const string ContentType = "Content-Type";
                public const string Date = "Date";
                public const string ETag = "ETag";
                public const string Expect = "Expect";
                public const string Expires = "Expires";
                public const string IfModifiedSince = "If-Modified-Since";
                public const string IfRange = "If-Range";
                public const string IfUnmodifiedSince = "If-Unmodified-Since";
                public const string LastModified = "Last-Modified";
                public const string Location = "Location";
                public const string Range = "Range";
                public const string Upgrade = "Upgrade";
                public const string Vary = "Vary";
                public const string PoweredBy = "X-Powered-By";
            }

            public class StatusCodes
            {
                public class Informational
                {
                    public const int Continue = 100;
                    public const int SwitchingProtocols = 101;
                }

                public class Successful
                {
                    public const int Ok = 200;
                    public const int Created = 201;
                    public const int Accepted = 202;
                    public const int NonAuthoritativeInformation = 203;
                    public const int NoContent = 204;
                    public const int ResetContent = 205;
                    public const int PartialContent = 206;
                }

                public class Redirection
                {
                    public const int MultipleChoices = 300;
                    public const int MovedPermanently = 301;
                    public const int Found = 302;
                    public const int SeeOther = 303;
                    public const int NotModified = 304;
                    public const int UseProxy = 305;
                    public const int TemporaryRedirect = 307;
                }

                public class ClientError
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

                public class Error
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

        public class Owin
        {
            #region Owin itself

            public const string OwinPrefix = "owin.";
            public const string Version = OwinPrefix + "Version";
            public const string CallCancelled = OwinPrefix + "CallCancelled";

            public class Request
            {
                public const string Prefix = OwinPrefix + "Request";
                public const string Body = Prefix + "Body";
                public const string Headers = Prefix + "Headers";
                public const string Method = Prefix + "Method";
                public const string Path = Prefix + "Path";
                public const string PathBase = Prefix + "PathBase";
                public const string Protocol = Prefix + "Protocol";
                public const string QueryString = Prefix + "QueryString";
                public const string Scheme = Prefix + "Scheme";
            }

            public class Response
            {
                public const string Prefix = OwinPrefix + "Response";
                public const string Body = Prefix + "Body";
                public const string Headers = Prefix + "Headers";
                public const string StatusCode = Prefix + "StatusCode";
                public const string ReasonPhrase = Prefix + "ReasonPhrase";
                public const string Protocol = Prefix + "Protocol";
            }

            #endregion

            public class Common
            {
                public class Ssl
                {
                    public const string Prefix = "ssl.";
                    public const string ClientCertificate = Prefix + "ClientCertificate";
                }

                public class Server
                {
                    public const string Prefix = "server.";
                    public const string RemoteIpAddress = Prefix + "RemoteIpAddress";
                    public const string RemotePort = Prefix + "RemotePort";
                    public const string LocalIpAddress = Prefix + "LocalIpAddress";
                    public const string LocalPort = Prefix + "LocalPort";
                    public const string IsLocal = Prefix + "IsLocal";
                    public const string Capabilities = Prefix + "Capabilities";
                    public const string OnSendingHeaders = Prefix + "OnSendingHeaders";
                }

                public class Host
                {
                    public const string Prefix = "host.";
                    public const string TraceOutput = Prefix + "TraceOutput";
                    public const string Addresses = Prefix + "Addresses";
                }
            }

            public class SendFile
            {
                public const string Prefix = "sendfile.";
                public const string Version = Prefix + "Version";
                public const string Support = Prefix + "Support";
                public const string Concurrency = Prefix + "Concurrency";
                public const string SendAsync = Prefix + "SendAsync";
            }

            public class WebSocket
            {
                public const string Prefix = "websocket.";
                public const string Version = Prefix + "Version";
                public const string Accept = Prefix + "Accept";
                public const string SendAsync = Prefix + "SendAsync";
                public const string ReceiveAsync = Prefix + "ReceiveAsync";
                public const string CloseAsync = Prefix + "CloseAsync";
                public const string CallCancelled = Prefix + "CallCancelled";
                public const string ClientCloseStatus = Prefix + "ClientCloseStatus";
                public const string ClientCloseDescription = Prefix + "ClientCloseDescription";
            }

            public class Opaque
            {
                public const string Prefix = "opaque.";
                public const string Version = Prefix + "Version";
                public const string Upgrade = Prefix + "Upgrade";
                public const string Stream = Prefix + "Stream";
                public const string CallCancelled = Prefix + "CallCancelled";
            }
        }
    }
}
