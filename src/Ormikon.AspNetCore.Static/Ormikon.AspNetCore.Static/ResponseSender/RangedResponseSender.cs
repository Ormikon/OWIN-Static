using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;
using Ormikon.AspNetCore.Static.Wrappers.Headers;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal class RangedResponseSender : ResponseSenderBase
    {
        private const string AllowedRange = "bytes";

        private static void SendRequestedRangeNotSatisfiable(IWrappedContext ctx, Stream responseStream)
        {
            responseStream.Close();
            var resp = StaticResponse.HttpStatus(Constants.Http.StatusCodes.ClientError.RequestedRangeNotSatisfiable);
            SetResponseHeaders(resp, ctx.Response);
        }

        private static long GetRangeLength(HttpRange range, long contentLength, out long start, out long end)
        {
            if (contentLength == 0)
            {
                start = end = 0;
                return 0;
            }
            start = range.Start ?? 0;
            end = range.End ?? (contentLength - 1);

            return end - start + 1;
        }

        private static async Task SendRangeAsync(HttpRange range, IStaticResponse response, Stream responseStream, IWrappedContext ctx, CancellationToken cancellationToken)
        {
            long? length = response.Headers.ContentLength.Value;
            if (!length.HasValue || length.Value == 0)
            {
                await SendFullResponseAsync(response, responseStream, ctx, cancellationToken);
                return;
            }

// ReSharper disable PossibleInvalidOperationException
            long rangeLength = GetRangeLength(range, length.Value, out var start, out var end);
// ReSharper restore PossibleInvalidOperationException
            if (start < 0 || start >= length.Value || end >= length.Value || rangeLength <= 0)
            {
                SendRequestedRangeNotSatisfiable(ctx, responseStream);
                return;
            }

            SetResponseHeaders(Constants.Http.StatusCodes.Successful.PartialContent, response.Headers, ctx.Response,
                               Constants.Http.Headers.ContentLength);
            ctx.Response.Headers.ContentRange.Range = new HttpContentRange(start, end, length.Value);
            ctx.Response.Headers.ContentLength.Value = rangeLength;

            await SendStreamAsync(responseStream, ctx.Response.Body, start, rangeLength, cancellationToken);
        }

        private static bool IfRange(HttpIfRangeHeader ifRange, string respETag, DateTimeOffset? lastModified)
        {
            if (string.IsNullOrEmpty(respETag) && !lastModified.HasValue)
                return false;
            var dateValue = ifRange.Value;
            if (dateValue.HasValue)
            {
                if (!lastModified.HasValue)
                    return false;
                return dateValue.Value >= lastModified.Value;
            }
            string entity = ifRange.Entity;
            if (string.IsNullOrEmpty(respETag) || string.IsNullOrEmpty(entity))
                return false;
            return string.CompareOrdinal(respETag, entity) == 0;
        }

        protected override async Task SendAsyncInternal(IStaticResponse response, Stream responseStream, IWrappedContext ctx, CancellationToken cancellationToken)
        {
            ctx.Response.Headers.AcceptRanges.AddAcceptValue(AllowedRange);
            var rangeHeader = ctx.Request.Headers.Range;
            if (!rangeHeader.Available)
            {
                await SendFullResponseAsync(response, responseStream, ctx, cancellationToken);
                return;
            }
            var range = rangeHeader.Range;
            if (!range.Valid)
            {
                SendRequestedRangeNotSatisfiable(ctx, responseStream);
                return;
            }
            var ifRange = ctx.Request.Headers.IfRange;
            if (ifRange.Available && !IfRange(ifRange, response.Headers.ETag.Value, response.Headers.LastModified.Value))
            {
                await SendFullResponseAsync(response, responseStream, ctx, cancellationToken);
                return;
            }

            await SendRangeAsync(range, response, responseStream, ctx, cancellationToken);
        }
    }
}
