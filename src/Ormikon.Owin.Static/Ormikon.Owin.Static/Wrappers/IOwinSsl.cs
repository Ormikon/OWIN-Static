using System;
using System.Security.Cryptography.X509Certificates;

namespace Ormikon.Owin.Static
{
    internal interface IOwinSsl
    {
        X509Certificate ClientCertificate { get; }
    }
}

