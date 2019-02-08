using System.IO;
using System.Threading.Tasks;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal abstract class CompressedResponseSender : ResponseSenderBase
    {
        protected abstract Stream WrapToCompressedStream(Stream outputStream);

        protected override Task SendAsyncInternal(IStaticResponse response, Stream responseStream, IWrappedContext ctx)
        {
            long? length = response.Headers.ContentLength.Value;
            if (!length.HasValue || length.Value == 0)
            {
                return SendFullResponse(response, responseStream, ctx);
            }

            SetResponseHeaders(response, ctx.Response, Constants.Http.Headers.ContentLength);
            ctx.Response.Headers.ContentEncoding.Value = CompressionMethod;

            if (!IsBodyRequested(ctx.Request.Method))
                return Task.FromResult<object>(null);

            var compressedStream = WrapToCompressedStream(ctx.Response.Body);
            return SendStreamAsync(responseStream, compressedStream, ctx.CallCancelled)
                .ContinueWith(
                    task =>
                        {
                            task.Wait(ctx.CallCancelled);
                            return compressedStream.FlushAsync(ctx.CallCancelled);
                        }, TaskContinuationOptions.ExecuteSynchronously)
                .Unwrap()
                .ContinueWith(
                    task =>
                        {
                            compressedStream.Close();
                            task.Wait(ctx.CallCancelled);
                        });
        }

        protected abstract string CompressionMethod { get; }
    }
}
