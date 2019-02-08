using System.IO;
using System.IO.Compression;

namespace Ormikon.AspNetCore.Static.ResponseSender
{
    internal class DeflatedResponseSender : CompressedResponseSender
    {
        public const string DeflatedCompressionMethod = "deflate";

        protected override Stream WrapToCompressedStream(Stream outputStream)
        {
            return new DeflateStream(outputStream, CompressionMode.Compress, true);
        }

        protected override string CompressionMethod => DeflatedCompressionMethod;
    }
}
