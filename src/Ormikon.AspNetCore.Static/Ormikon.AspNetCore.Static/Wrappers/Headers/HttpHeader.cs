using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal abstract class HttpHeader : IHttpHeader
    {
        protected const string SplitString = ",";

        private readonly IDictionary<string, StringValues> headers;
        private readonly string code;

        protected HttpHeader(IDictionary<string, StringValues> headers, string code)
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
            if (values.Count == 0)
                return "";
            return code + ": " + string.Join(SplitString, values);
        }

        #region Helpers

        protected string GetSingleValue()
        {
            var values = Values;
            return values.Count == 0 ? null : values[0];
        }

        protected void SetSingleValue(string value)
        {
            if (value == null)
                Clear();
            else
                Values = value;
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

        public StringValues Values
        {
            get
            {
                StringValues result;
                if (headers.TryGetValue(code, out result))
                    return result;
                return StringValues.Empty;
            }
            protected set
            {
                if (value.Count == 0)
                    Clear();
                else
                    headers[code] = value;
            }
        }

        #endregion
    }
}

