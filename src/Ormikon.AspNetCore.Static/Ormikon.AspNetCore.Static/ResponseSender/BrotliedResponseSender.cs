using System.IO;
using System.IO.Compression;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal class BrotliResponseSender : CompressedResponseSender
    {
        public const string BrotliCompressionMethod = "br";

        protected override Stream WrapToCompressedStream(Stream outputStream)
        {
            return new GZipStream(outputStream, CompressionMode.Compress, true);
        }

        protected override string CompressionMethod => BrotliCompressionMethod;
    }
}
