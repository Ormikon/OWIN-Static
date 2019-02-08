using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpAcceptHeader : HttpPropertyHeader
    {
        public HttpAcceptHeader(IDictionary<string, StringValues> headers, string code)
            : base(headers, code)
        {
        }

        protected HttpAcceptHeaderValue[] GetAcceptValues()
        {
            return GetEnumValues().Select(v => new HttpAcceptHeaderValue(v)).ToArray();
        }

        protected void SetAcceptValues(ICollection<HttpAcceptHeaderValue> values)
        {
            SetEnumValues(values?.Select(av => av.ToString()).ToList());
        }

        public void AddAcceptValue(string acceptValue)
        {
            AddAcceptValue(new HttpAcceptHeaderValue(acceptValue));
        }

        public void AddAcceptValue(string acceptValue, float qualityFactor)
        {
            AddAcceptValue(new HttpAcceptHeaderValue(acceptValue, qualityFactor));
        }

        public void AddAcceptValue(HttpAcceptHeaderValue acceptValue)
        {
            var values = new List<HttpAcceptHeaderValue>(GetAcceptValues()) {acceptValue};
            SetSingleValue(string.Join(SplitString, values.Select(v => v.ToString())));
        }

        public HttpAcceptHeaderValue[] AcceptValues => GetAcceptValues();
    }

    internal class HttpAcceptHeaderValue : HttpPropertyHeaderValue
    {
        private const string QualityFactorProp = "q";

        public HttpAcceptHeaderValue(string acceptValue)
            : base (acceptValue)
        {
        }

        public HttpAcceptHeaderValue(string acceptValue, float qualityFactor)
            : this(acceptValue)
        {
            QualityFactor = qualityFactor;
        }

        public float QualityFactor
        {
            get
            {
                string strValue = this[QualityFactorProp];
                if (string.IsNullOrEmpty(strValue))
                    return 1;
                return float.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 1;
            }
            set => this[QualityFactorProp] = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}

