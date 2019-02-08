using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal abstract class CompressedResponseSender : ResponseSenderBase
    {
        protected abstract Stream WrapToCompressedStream(Stream outputStream);

        protected override async Task SendAsyncInternal(IStaticResponse response, Stream responseStream, IWrappedContext ctx, CancellationToken cancellationToken)
        {
            long? length = response.Headers.ContentLength.Value;
            if (!length.HasValue || length.Value == 0)
            {
                await SendFullResponseAsync(response, responseStream, ctx, cancellationToken);
                return;
            }

            SetResponseHeaders(response, ctx.Response, Constants.Http.Headers.ContentLength);
            ctx.Response.Headers.ContentEncoding.Value = CompressionMethod;

            if (!IsBodyRequested(ctx.Request.Method))
                return;

            var compressedStream = WrapToCompressedStream(ctx.Response.Body);

            await SendStreamAsync(responseStream, compressedStream, cancellationToken);
            await compressedStream.FlushAsync(cancellationToken);
            compressedStream.Close();
        }

        protected abstract string CompressionMethod { get; }
    }
}
