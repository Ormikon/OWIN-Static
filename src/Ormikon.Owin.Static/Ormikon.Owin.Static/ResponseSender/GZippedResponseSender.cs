using System.IO;
using System.IO.Compression;

namespace Ormikon.Owin.Static.ResponseSender
{
    internal class GZippedResponseSender : CompressedResponseSender
    {
        protected override Stream WrapToCompressedStream(Stream outputStream)
        {
            return new GZipStream(outputStream, CompressionMode.Compress, true);
        }

        protected override string CompressionMethod
        {
            get { return "gzip"; }
        }
    }
}
