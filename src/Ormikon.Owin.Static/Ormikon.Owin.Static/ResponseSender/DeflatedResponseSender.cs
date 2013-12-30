using System.IO;
using System.IO.Compression;

namespace Ormikon.Owin.Static.ResponseSender
{
    internal class DeflatedResponseSender : CompressedResponseSender
    {
        protected override Stream WrapToCompressedStream(Stream outputStream)
        {
            return new DeflateStream(outputStream, CompressionMode.Compress, true);
        }

        protected override string CompressionMethod
        {
            get { return "deflate"; }
        }
    }
}
