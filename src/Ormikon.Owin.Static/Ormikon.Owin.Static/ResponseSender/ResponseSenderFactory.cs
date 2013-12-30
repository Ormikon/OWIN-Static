using System;
using System.Linq;
using Ormikon.Owin.Static.Filters;
using Ormikon.Owin.Static.Responses;
using Ormikon.Owin.Static.Wrappers;

namespace Ormikon.Owin.Static.ResponseSender
{
    internal class ResponseSenderFactory : IResponseSenderFactory
    {
        private readonly IFilter compressedContentFilter;

        public ResponseSenderFactory(IFilter compressedContentFilter)
        {
            this.compressedContentFilter = compressedContentFilter;
        }

        private static string GetClientCompressionMethod(IOwinContext context)
        {
            var acceptedEncodingHeader = context.Request.Headers.AcceptEncoding;
            if (!acceptedEncodingHeader.Available)
                return null;
            var encodings = acceptedEncodingHeader.AcceptValues.OrderByDescending(av => av.QualityFactor).Select(av => av.Value);
            foreach (var encoding in encodings)
            {
                if (string.CompareOrdinal(encoding, "*") == 0)
                    return "gzip";
                if (string.Compare(encoding, "gzip", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(encoding, "deflate", StringComparison.OrdinalIgnoreCase) == 0)
                    return encoding.ToLowerInvariant();
            }
            return null;
        }

        private bool CanResponseBeCompressed(IStaticResponse response)
        {
            if (!compressedContentFilter.IsActive() || response.StatusCode != Constants.Http.StatusCodes.Successful.Ok)
                return false;
            var propValues = response.Headers.ContentType.PropertyValues;
            if (propValues.Length == 0)
                return false;
            if (compressedContentFilter.Contains(propValues[0].Value))
            {
                return true;
            }
            return false;
        }

        public IResponseSender CreateSenderFor(IStaticResponse response, IOwinContext context)
        {
            if (!CanResponseBeCompressed(response))
                return new RangedResponseSender();
            string compressionMethod = GetClientCompressionMethod(context);
            if (compressionMethod == null)
                return new RangedResponseSender();
            if (string.CompareOrdinal(compressionMethod, "gzip") == 0)
                return new GZippedResponseSender();
            return new DeflatedResponseSender();
        }
    }
}
