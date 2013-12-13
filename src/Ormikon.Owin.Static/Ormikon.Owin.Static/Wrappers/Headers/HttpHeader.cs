using System;
using System.Collections.Generic;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal abstract class HttpHeader : IHttpHeader
    {
        protected const string SplitString = ",";

        private readonly IDictionary<string, string[]> headers;
        private readonly string code;

        protected HttpHeader(IDictionary<string, string[]> headers, string code)
        {
            if (headers == null)
                throw new ArgumentNullException("headers");
            if (code == null)
                throw new ArgumentNullException("code");
            if (code.Length == 0)
                throw new ArgumentException("Header code cannot be empty.", "code");
            this.headers = headers;
            this.code = code;
        }

        public override string ToString()
        {
            var values = Values;
            if (values == null)
                return "";
            return code + ": " + string.Join(SplitString, values);
        }

        #region Helpers

        protected string GetSingleValue()
        {
            var values = Values;
            return values == null || values.Length == 0 ? null : values[0];
        }

        protected void SetSingleValue(string value)
        {
            if (value == null)
                Clear();
            else
                Values = new [] { value };
        }

        #endregion

        #region IHttpHeader implementation

        public void Clear()
        {
            if (headers.ContainsKey(code))
                headers.Remove(code);
        }

        public bool Available
        {
            get
            {
                return headers.ContainsKey(code);
            }
        }

        public string[] Values
        {
            get
            {
                string[] result;
                if (headers.TryGetValue(code, out result))
                    return result;
                return null;
            }
            protected set
            {
                if (value == null)
                    Clear();
                else
                    headers[code] = value;
            }
        }

        #endregion
    }
}

