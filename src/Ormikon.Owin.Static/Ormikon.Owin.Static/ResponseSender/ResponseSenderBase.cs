using System;
using Ormikon.Owin.Static.Responses;
using System.IO;
using Ormikon.Owin.Static.Wrappers;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.ResponseSender
{
    internal abstract class ResponseSenderBase : IResponseSender
    {
        private enum PreconditionResult
        {
            Continue,
            NotModified,
            PrecondutionFailed
        }


        private const int CopyBufferSize = 32768;

        protected static Task SendStreamAsync(Stream from, Stream to)
        {
            return from.CopyToAsync(to, CopyBufferSize)
                    .ContinueWith(
                        task =>
                        {
                            from.Close();
                            task.Wait();
                        }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private static Task ReadAsync(byte[] buffer, Stream stream, long length)
        {
            int copyLength = (int)Math.Min(CopyBufferSize, length);
            if (copyLength <= 0)
                return Task.FromResult<object>(null);
            return stream.ReadAsync(buffer, 0, copyLength)
                    .ContinueWith(
                        task =>
                        {
                            task.Wait();
                            if (task.Result <= 0 || task.Result < copyLength)
                                return Task.FromResult<object>(null);
                            return ReadAsync(buffer, stream, length - task.Result);
                        }, TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        private static Task SeekAsync(Stream stream, long length)
        {
            if (length == 0)
            {
                return Task.FromResult<object>(null);
            }
            if (stream.CanSeek)
            {
                stream.Seek(length, SeekOrigin.Current);
                return Task.FromResult<object>(null);
            }
            var buffer = new byte[Math.Min(CopyBufferSize, length)];
            return ReadAsync(buffer, stream, length);
        }

        private static Task SendStreamAsync(byte[] buffer, Stream from, Stream to, long length)
        {
            int copyLength = (int)Math.Min(CopyBufferSize, length);
            if (copyLength <= 0)
                return Task.FromResult<object>(null);
            return from.ReadAsync(buffer, 0, copyLength)
                    .ContinueWith(
                        task =>
                        {
                            task.Wait();
                            if (task.Result <= 0)
                                return Task.FromResult<object>(null);
                            return to.WriteAsync(buffer, 0, task.Result)
                                    .ContinueWith(
                                        subTask =>
                                        {
                                            subTask.Wait();
                                            return SendStreamAsync(buffer, from, to, length - task.Result);
                                        }, TaskContinuationOptions.ExecuteSynchronously)
                                    .Unwrap();
                        }, TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        protected static Task SendStreamAsync(Stream from, Stream to, long start, long length)
        {
            if (length <= 0)
            {
                from.Close();
                return Task.FromResult<object>(null);
            }
            if (start < 0)
                start = 0;
            return SeekAsync(from, start)
                    .ContinueWith(
                        task =>
                        {
                            task.Wait();
                            var buffer = new byte[Math.Min(CopyBufferSize, length)];
                            return SendStreamAsync(buffer, from, to, length);
                        })
                    .Unwrap();
        }

        protected static void SetResponseHeaders(IStaticResponse staticResponse, IOwinResponse response)
        {
            response.StatusCode = staticResponse.StatusCode;
            if (!string.IsNullOrEmpty(staticResponse.ReasonPhrase))
                response.ReasonPhrase = staticResponse.ReasonPhrase;
            staticResponse.Headers.CopyTo(response.Headers);
        }

        protected static bool IsBodyRequested(string method)
        {
            return string.Compare(method, Constants.Http.Methods.Get, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static Task SendCacheStatus(IStaticResponse response, Stream responseStream, IOwinContext ctx, int statusCode)
        {
            responseStream.Close();
            SetResponseHeaders(response, ctx.Response);
            ctx.Response.StatusCode = statusCode;
            return Task.FromResult<object>(null);
        }

        private static Task SendPreconditionResult(PreconditionResult result, IStaticResponse response, Stream responseStream, IOwinContext ctx)
        {
            var cacheStatus = result == PreconditionResult.PrecondutionFailed
                                  ? Constants.Http.StatusCodes.ClientError.PreconditionFailed
                                  : Constants.Http.StatusCodes.Redirection.NotModified;
            return SendCacheStatus(response, responseStream, ctx, cacheStatus);
        }

        private static PreconditionResult IfMatch(IOwinContext ctx, string respETag)
        {
            var ifMatch = ctx.Request.Headers.IfMatch;
            if (ifMatch.Available)
            {
                var eTags = ifMatch.EnumValues;
                if (eTags == null || eTags.Length == 0)
                {
                    return PreconditionResult.PrecondutionFailed;
                }
                if (string.IsNullOrEmpty(respETag))
                {
                    return PreconditionResult.PrecondutionFailed;
                }
                bool tagFound = false;
                foreach(var eTag in eTags)
                {
                    if (string.CompareOrdinal(eTag, "*") == 0 || string.CompareOrdinal(eTag, respETag) == 0)
                    {
                        tagFound = true;
                        break;
                    }
                }
                if (!tagFound)
                {
                    return PreconditionResult.PrecondutionFailed;
                }
            }

            return PreconditionResult.Continue;
        }

        private static PreconditionResult IfNoneMatch(IOwinContext ctx, string respETag)
        {
            var ifNoneMatch = ctx.Request.Headers.IfNoneMatch;
            if (ifNoneMatch.Available)
            {
                var eTags = ifNoneMatch.EnumValues;
                if (eTags == null || eTags.Length == 0)
                {
                    return PreconditionResult.PrecondutionFailed;
                }
                bool tagFound = false;
                foreach (var eTag in eTags)
                {
                    if (string.CompareOrdinal(eTag, "*") == 0 || string.CompareOrdinal(eTag, respETag) == 0)
                    {
                        tagFound = true;
                        break;
                    }
                }
                if (tagFound)
                    return PreconditionResult.NotModified;
            }

            return PreconditionResult.Continue;
        }

        private static PreconditionResult IfModifiedSince(IOwinContext ctx, DateTimeOffset lastModified)
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

        private static PreconditionResult IfUnmodifiedSince(IOwinContext ctx, DateTimeOffset lastModified)
        {
            var dateCheck = ctx.Request.Headers.IfModifiedSince.Value;
            if (dateCheck.HasValue)
            {
                if (dateCheck < lastModified)
                    return PreconditionResult.PrecondutionFailed;
            }

            return PreconditionResult.Continue;
        }

        private static Task ProcessCacheHeaders(IStaticResponse response, Stream responseStream, IOwinContext ctx)
        {
            PreconditionResult preconditionResult;

            var respETag = response.Headers.ETag.Value;
            if (!string.IsNullOrEmpty(respETag))
            {
                preconditionResult = IfMatch(ctx, respETag);
                if (preconditionResult != PreconditionResult.Continue)
                {
                    return SendPreconditionResult(preconditionResult, response, responseStream, ctx);
                }
                preconditionResult = IfNoneMatch(ctx, respETag);
                if (preconditionResult != PreconditionResult.Continue)
                {
                    return SendPreconditionResult(preconditionResult, response, responseStream, ctx);
                }
            }

            var lastModified = response.Headers.LastModified.Value;
            if (lastModified.HasValue)
            {
                preconditionResult = IfModifiedSince(ctx, lastModified.Value);
                if (preconditionResult != PreconditionResult.Continue)
                {
                    return SendPreconditionResult(preconditionResult, response, responseStream, ctx);
                }
                preconditionResult = IfUnmodifiedSince(ctx, lastModified.Value);
                if (preconditionResult != PreconditionResult.Continue)
                {
                    return SendPreconditionResult(preconditionResult, response, responseStream, ctx);
                }
            }

            return null;
        }

        protected abstract Task SendAsyncInternal(IStaticResponse response, Stream responseStream, IOwinContext ctx);

        #region IResponseSender implementation

        public Task SendAsync(IStaticResponse response, Stream responseStream, IOwinContext ctx)
        {
            if (response.StatusCode != Constants.Http.StatusCodes.Successful.Ok)
            {
                SetResponseHeaders(response, ctx.Response);
                return SendStreamAsync(responseStream, ctx.Response.Body);
            }
            return ProcessCacheHeaders(response, responseStream, ctx) ?? SendAsyncInternal(response, responseStream, ctx);
        }

        #endregion
    }
}

