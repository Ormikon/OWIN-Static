using System.Collections.Generic;
using System.IO;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static.Wrappers
{
    internal class OwinHost : IOwinHost
    {
        private readonly IDictionary<string, object> data;

        public OwinHost(IDictionary<string, object> data)
        {
            this.data = data;
        }

        #region IOwinHost implementation

        public TextWriter TraceOutput {
            get
            {
                return data.Get<TextWriter>(Constants.Owin.Common.Host.TraceOutput);
            }
        }

        public IList<IDictionary<string, object>> Addresses {
            get
            {
                return data.Get<IList<IDictionary<string, object>>>(Constants.Owin.Common.Host.Addresses);
            }
        }

        #endregion
    }
}

