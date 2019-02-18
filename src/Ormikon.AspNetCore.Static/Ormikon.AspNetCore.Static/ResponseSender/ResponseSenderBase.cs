using System;
using System.Linq;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;
using Ormikon.AspNetCore.Static.Wrappers.Headers;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal abstract class ResponseSenderBase : IResponseSender
    {
        private enum PreconditionResult
        {
            Continue,
            NotModified,
            PreconditionFailed
        }


        private const int CopyBufferSize = 32768;

        protected static async Task SendStreamAsync(Stream from, Stream to, CancellationToken cancellationToken)
        {
            await from.CopyToAsync(to, CopyBufferSize, cancellationToken);
            from.Close();
        }

        private static async Task ReadAsync(byte[] buffer, Stream stream, long length, CancellationToken cancellationToken)
        {
            while (length > 0)
            {
                var copyLength = (int) Math.Min(CopyBufferSize, length);
                length -= await stream.ReadAsync(buffer, 0, copyLength, cancellationToken);
            }
        }

        private static async Task SeekAsync(Stream stream, long length, CancellationToken cancellationToken)
        {
            if (length == 0)
            {
                return;
            }
            if (stream.CanSeek)
            {
                stream.Seek(length, SeekOrigin.Current);
                return;
            }
            var buffer = new byte[Math.Min(CopyBufferSize, length)];
            await ReadAsync(buffer, stream, length, cancellationToken);
        }

        private static async Task SendStreamAsync(byte[] buffer, Stream from, Stream to, long length, CancellationToken cancellationToken)
        {
            while (length > 0)
            {
                var copyLength = (int) Math.Min(CopyBufferSize, length);
                int bytesRead = await from.ReadAsync(buffer, 0, copyLength, cancellationToken);
                length -= bytesRead;
                await to.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            }
        }

        protected static async Task SendStreamAsync(Stream from, Stream to, long start, long length, CancellationToken cancellationToken)
        {
            if (length <= 0)
            {
                from.Close();
                return;
            }
            if (start < 0)
                start = 0;

            await SeekAsync(from, start, cancellationToken);
            var buffer = new byte[Math.Min(CopyBufferSize, length)];
            await SendStreamAsync(buffer, from, to, length, cancellationToken);
        }

        protected static void SetResponseHeaders(int statusCode, string reasonPhrase, IHttpHeaders headers,
                                                 IWrappedResponse response, params string[] except)
        {
            response.StatusCode = statusCode;
            if (!string.IsNullOrEmpty(reasonPhrase))
                response.ReasonPhrase = reasonPhrase;
            headers.CopyTo(response.Headers, except);
        }

        protected static void SetResponseHeaders(int statusCode, IHttpHeaders headers, IWrappedResponse response, params string[] except)
        {
            SetResponseHeaders(statusCode, null, headers, response, except);
        }

        protected static void SetResponseHeaders(IStaticResponse staticResponse, IWrappedResponse response, params string[] except)
        {
            SetResponseHeaders(staticResponse.StatusCode, staticResponse.ReasonPhrase, staticResponse.Headers, response, except);
        }

        protected static async Task SendFullResponseAsync(IStaticResponse response, Stream responseStream, IWrappedContext ctx, CancellationToken cancellationToken)
        {
            SetResponseHeaders(response, ctx.Response);
            await SendStreamAsync(responseStream, ctx.Response.Body, cancellationToken);
        }

        protected static bool IsBodyRequested(string method)
        {
            return string.Compare(method, Constants.Http.Methods.Get, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static void SendCacheStatus(IStaticResponse response, Stream responseStream, IWrappedContext ctx, int statusCode)
        {
            responseStream.Close();
            SetResponseHeaders(statusCode, response.Headers, ctx.Response, Constants.Http.Headers.ContentLength);
        }

        private static void SendPreconditionResult(PreconditionResult result, IStaticResponse response, Stream responseStream, IWrappedContext ctx)
        {
            var cacheStatus = result == PreconditionResult.PreconditionFailed
                                  ? Constants.Http.StatusCodes.ClientError.PreconditionFailed
                                  : Constants.Http.StatusCodes.Redirection.NotModified;
            SendCacheStatus(response, responseStream, ctx, cacheStatus);
        }

        private static PreconditionResult IfMatch(IWrappedContext ctx, string respETag)
        {
            var ifMatch = ctx.Request.Headers.IfMatch;
            if (ifMatch.Available)
            {
                var eTags = ifMatch.EnumValues;
                if (eTags == null || eTags.Length == 0)
                {
                    return PreconditionResult.PreconditionFailed;
                }
                if (string.IsNullOrEmpty(respETag))
                {
                    return PreconditionResult.PreconditionFailed;
                }
                bool tagFound = eTags.Any(eTag => string.CompareOrdinal(eTag, "*") == 0 || string.CompareOrdinal(eTag, respETag) == 0);
                if (!tagFound)
                {
                    return PreconditionResult.PreconditionFailed;
                }
            }

            return PreconditionResult.Continue;
        }

        private static PreconditionResult IfNoneMatch(IWrappedContext ctx, string respETag)
        {
            var ifNoneMatch = ctx.Request.Headers.IfNoneMatch;
            if (ifNoneMatch.Available)
            {
                var eTags = ifNoneMatch.EnumValues;
                if (eTags == null || eTags.Length == 0)
                {
                    return PreconditionResult.PreconditionFailed;
                }
                bool tagFound = eTags.Any(eTag => string.CompareOrdinal(eTag, "*") == 0 || string.CompareOrdinal(eTag, respETag) == 0);
                if (tagFound)
                    return PreconditionResult.NotModified;
            }

            return PreconditionResult.Continue;
        }

        private static PreconditionResult IfModifiedSince(IWrappedContext ctx, DateTimeOffset lastModified)
        {
            if (ctx.Request.Headers.IfNoneMatch.Available)
                return PreconditionResult.Continue;
            var dateCheck = ctx.Request.Headers.IfModifiedSince.Value;
            if (dateCheck.HasValue)
            {
                if (dateCheck >= lastModified)
                    return PreconditionResult.NotModified;
            }

            return PreconditionResult.Continue;
        }

        private static PreconditionResult IfUnmodifiedSince(IWrappedContext ctx, DateTimeOffset lastModified)
        {
            var dateCheck = ctx.Request.Headers.IfModifiedSince.Value;
            if (dateCheck.HasValue)
            {
                if (dateCheck < lastModified)
                    return PreconditionResult.PreconditionFailed;
            }

            return PreconditionResult.Continue;
        }

        private static bool ProcessCacheHeaders(IStaticResponse response, Stream responseStream, IWrappedContext ctx)
        {
            PreconditionResult preconditionResult;

            var respETag = response.Headers.ETag.Value;
            if (!string.IsNullOrEmpty(respETag))
            {
                preconditionResult = IfMatch(ctx, respETag);
                if (preconditionResult == PreconditionResult.NotModified)
                {
                    SendPreconditionResult(preconditionResult, response, responseStream, ctx);
                    return true;
                }
                preconditionResult = IfNoneMatch(ctx, respETag);
                if (preconditionResult == PreconditionResult.NotModified)
                {
                    SendPreconditionResult(preconditionResult, response, responseStream, ctx);
                    return true;
                }
            }

            var lastModified = response.Headers.LastModified.Value;
            if (lastModified.HasValue)
            {
                preconditionResult = IfModifiedSince(ctx, lastModified.Value);
                if (preconditionResult == PreconditionResult.NotModified)
                {
                    SendPreconditionResult(preconditionResult, response, responseStream, ctx);
                    return true;
                }
                preconditionResult = IfUnmodifiedSince(ctx, lastModified.Value);
                if (preconditionResult == PreconditionResult.NotModified)
                {
                    SendPreconditionResult(preconditionResult, response, responseStream, ctx);
                    return true;
                }
            }

            return false;
        }

        protected abstract Task SendAsyncInternal(IStaticResponse response, Stream responseStream, IWrappedContext ctx, CancellationToken cancellationToken);

        #region IResponseSender implementation

        public async Task SendAsync(IStaticResponse response, Stream responseStream, IWrappedContext context, CancellationToken cancellationToken)
        {
            if (response.StatusCode != Constants.Http.StatusCodes.Successful.Ok)
            {
                await SendFullResponseAsync(response, responseStream, context, cancellationToken);
                return;
            }

            if (!ProcessCacheHeaders(response, responseStream, context))
            {
                await SendAsyncInternal(response, responseStream, context, cancellationToken);
            }
        }

        #endregion
    }
}

