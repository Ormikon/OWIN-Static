using System.Security.Cryptography.X509Certificates;

namespace Ormikon.Owin.Static.Wrappers
{
    internal interface IOwinSsl
    {
        X509Certificate ClientCertificate { get; }
    }
}

