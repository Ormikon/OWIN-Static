using System;
using System.Linq;
using Ormikon.AspNetCore.Static.Filters;
using Ormikon.AspNetCore.Static.Responses;
using Ormikon.AspNetCore.Static.Wrappers;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal class ResponseSenderFactory : IResponseSenderFactory
    {
        private readonly IFilter compressedContentFilter;

        public ResponseSenderFactory(IFilter compressedContentFilter)
        {
            this.compressedContentFilter = compressedContentFilter;
        }

        private static string GetClientCompressionMethod(IWrappedContext context)
        {
            var acceptedEncodingHeader = context.Request.Headers.AcceptEncoding;
            if (!acceptedEncodingHeader.Available)
                return null;
            var encodings = acceptedEncodingHeader.AcceptValues.OrderByDescending(av => av.QualityFactor).Select(av => av.Value);
            foreach (var encoding in encodings)
            {
                if (string.CompareOrdinal(encoding, "*") == 0)
                    return GZippedResponseSender.GZipCompressionMethod;
                if (string.Compare(encoding, GZippedResponseSender.GZipCompressionMethod, StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(encoding, DeflatedResponseSender.DeflatedCompressionMethod, StringComparison.OrdinalIgnoreCase) == 0)
                    return encoding.ToLowerInvariant();
            }
            return null;
        }

        private bool CanResponseBeCompressed(IStaticResponse response)
        {
            if (!compressedContentFilter.IsActive() || response.StatusCode != Constants.Http.StatusCodes.Successful.Ok)
                return false;
            var propValues = response.Headers.ContentType.PropertyValues;
            return propValues.Length != 0 && compressedContentFilter.Contains(propValues[0].Value);
        }

        public IResponseSender CreateSenderFor(IStaticResponse response, IWrappedContext context)
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
