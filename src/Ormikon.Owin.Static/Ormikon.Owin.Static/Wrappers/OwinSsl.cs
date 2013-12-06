using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static
{
    internal class OwinSsl : IOwinSsl
    {
        private readonly IDictionary<string, object> data;

        public OwinSsl(IDictionary<string, object> data)
        {
            this.data = data;
        }

        #region IOwinSsl implementation

        public X509Certificate ClientCertificate {
            get
            {
                return data.Get<X509Certificate>(Constants.Owin.Common.Ssl.ClientCertificate);
            }
        }

        #endregion
    }
}

