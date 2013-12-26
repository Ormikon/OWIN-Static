using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;
using Ormikon.Owin.Static.Wrappers.Headers;

namespace Ormikon.Owin.Static.ResponseSender
{
    internal class RangedResponseSender : ResponseSenderBase
    {
        private const string AllowedRange = "bytes";

        private static Task SendRequestedRangeNotSatisfable(IOwinContext ctx, Stream responseStream)
        {
            responseStream.Close();
            var resp = StaticResponse.HttpStatus(Constants.Http.StatusCodes.ClientError.RequestedRangeNotSatisfiable);
            SetResponseHeaders(resp, ctx.Response);
            return Task.FromResult<object>(null);
        }

        private static Task SendFull(IStaticResponse response, Stream responseStream, IOwinContext ctx)
        {
            SetResponseHeaders(response, ctx.Response);
            return SendStreamAsync(responseStream, ctx.Response.Body);
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
                SendFull(response, responseStream, ctx);
            }
            SetResponseHeaders(response, ctx.Response);
            ctx.Response.StatusCode = Constants.Http.StatusCodes.Successful.PartialContent;

            throw new NotImplementedException();
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
                return SendFull(response, responseStream, ctx);
            }
            var range = rangeHeader.Range;
            if (!range.Valid)
            {
                return SendRequestedRangeNotSatisfable(ctx, responseStream);
            }
            var ifRange = ctx.Request.Headers.IfRange;
            if (ifRange.Available && !IfRange(ifRange, response.Headers.ETag.Value, response.Headers.LastModified.Value))
            {
                return SendFull(response, responseStream, ctx);
            }

            return SendRange(range, response, responseStream, ctx);
        }
    }
}
