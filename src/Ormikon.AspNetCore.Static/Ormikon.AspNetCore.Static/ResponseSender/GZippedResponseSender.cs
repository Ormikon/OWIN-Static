using System.IO;
using System.IO.Compression;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal class GZippedResponseSender : CompressedResponseSender
    {
        public const string GZipCompressionMethod = "gzip";

        protected override Stream WrapToCompressedStream(Stream outputStream)
        {
            return new GZipStream(outputStream, CompressionMode.Compress, true);
        }

        protected override string CompressionMethod => GZipCompressionMethod;
    }
}
