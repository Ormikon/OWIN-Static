using System;
using System.IO;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;
using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.ResponseSender
{
    internal class RangedResponseSender : ResponseSenderBase
    {
        private const string AllowedRange = "bytes";

        private static Task SendRequestedRangeNotSatisfiable(IOwinContext ctx, Stream responseStream)
        {
            responseStream.Close();
            var resp = StaticResponse.HttpStatus(Constants.Http.StatusCodes.ClientError.RequestedRangeNotSatisfiable);
            SetResponseHeaders(resp, ctx.Response);
            return Task.FromResult<object>(null);
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

        private static Task SendRange(HttpRange range, IStaticResponse response, Stream responseStream, IOwinContext ctx)
        {
            long? length = response.Headers.ContentLength.Value;
            if (!length.HasValue || length.Value == 0)
            {
                SendFullResponse(response, responseStream, ctx);
            }

            long start, end;
// ReSharper disable PossibleInvalidOperationException
            long rangeLength = GetRangeLength(range, length.Value, out start, out end);
// ReSharper restore PossibleInvalidOperationException
            if (start < 0 || start >= length.Value || end >= length.Value || rangeLength <= 0)
            {
                return SendRequestedRangeNotSatisfiable(ctx, responseStream);
            }

            SetResponseHeaders(Constants.Http.StatusCodes.Successful.PartialContent, response.Headers, ctx.Response,
                               Constants.Http.Headers.ContentLength);
            ctx.Response.Headers.ContentRange.Range = new HttpContentRange(start, end, length.Value);
            ctx.Response.Headers.ContentLength.Value = rangeLength;

            return SendStreamAsync(responseStream, ctx.Response.Body, start, rangeLength, ctx.CallCancelled);
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

        protected override Task SendAsyncInternal(IStaticResponse response, Stream responseStream, IOwinContext ctx)
        {
            ctx.Response.Headers.AcceptRanges.AddAcceptValue(AllowedRange);
            var rangeHeader = ctx.Request.Headers.Range;
            if (!rangeHeader.Available)
            {
                return SendFullResponse(response, responseStream, ctx);
            }
            var range = rangeHeader.Range;
            if (!range.Valid)
            {
                return SendRequestedRangeNotSatisfiable(ctx, responseStream);
            }
            var ifRange = ctx.Request.Headers.IfRange;
            if (ifRange.Available && !IfRange(ifRange, response.Headers.ETag.Value, response.Headers.LastModified.Value))
            {
                return SendFullResponse(response, responseStream, ctx);
            }

            return SendRange(range, response, responseStream, ctx);
        }
    }
}
