using System;
using System.Net;
using System.Collections.Generic;
using Ormikon.Owin.Static.Extensions;

namespace Ormikon.Owin.Static
{
    internal class OwinServer : IOwinServer
    {
        private readonly IDictionary<string, object> data;

        public OwinServer(IDictionary<string, object> data)
        {
            this.data = data;
        }

        #region IOwinServer implementation

        public IPAddress RemoteIpAddress {
            get
            {
                string address = data.Get<string>(Constants.Owin.Common.Server.RemoteIpAddress);
                return string.IsNullOrEmpty(address) ? null : IPAddress.Parse(address);
            }
        }

        public int RemotePort {
            get
            {
                string port = data.Get<string>(Constants.Owin.Common.Server.RemotePort);
                return string.IsNullOrEmpty(port) ? 0 : int.Parse(port);
            }
        }

        public IPAddress LocalIpAddress {
            get
            {
                string address = data.Get<string>(Constants.Owin.Common.Server.LocalIpAddress);
                return string.IsNullOrEmpty(address) ? null : IPAddress.Parse(address);
            }
        }

        public int LocalPort {
            get
            {
                string port = data.Get<string>(Constants.Owin.Common.Server.LocalPort);
                return string.IsNullOrEmpty(port) ? 0 : int.Parse(port);
            }
        }

        public bool IsLocal {
            get
            {
                return data.Get<bool>(Constants.Owin.Common.Server.IsLocal);
            }
        }

        public IDictionary<string, object> Capabilities {
            get
            {
                return data.Get<IDictionary<string, object>>(Constants.Owin.Common.Server.Capabilities);
            }
        }

        public Action<Action<object>, object> OnSendingHeaders {
            get
            {
                return data.Get<Action<Action<object>, object>>(Constants.Owin.Common.Server.OnSendingHeaders);
            }
        }

        #endregion
    }
}

