using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ormikon.Owin.Static.Headers
{
    internal class HttpAcceptHeader : HttpStringHeader
    {
        private const string SplitString = ",";
        private static readonly char[] splitChar = new [] { ',' };

        public HttpAcceptHeader(IDictionary<string, string[]> headers, string code)
            : base(headers, code)
        {
        }

        protected HttpAcceptHeaderValue[] GetAcceptValues()
        {
            string stringValue = GetSingleValue();
            if (string.IsNullOrEmpty(stringValue))
            {
                return new HttpAcceptHeaderValue[0];
            }
            return stringValue.Split(splitChar, StringSplitOptions.RemoveEmptyEntries)
                    .Select(av => new HttpAcceptHeaderValue(av))
                    .ToArray();
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
            var values = new List<HttpAcceptHeaderValue>(GetAcceptValues());
            values.Add(acceptValue);
            SetSingleValue(string.Join(SplitString, values.Select(v => v.ToString())));
        }

        public HttpAcceptHeaderValue[] AcceptValues
        {
            get { return GetAcceptValues(); }
        }
    }

    internal class HttpAcceptHeaderValue
    {
        private readonly string acceptValue;
        private readonly float qualityFactor;

        public HttpAcceptHeaderValue (string acceptValue)
        {
            if (acceptValue == null)
                throw new ArgumentNullException("acceptValue");
            if (acceptValue.Length == 0)
                throw new ArgumentException("Accept header value cannot be empty", "acceptValue");
            this.acceptValue = TryExtractQualityFactor(acceptValue, out qualityFactor);
        }

        public HttpAcceptHeaderValue(string acceptValue, float qualityFactor)
        {
            if (acceptValue == null)
                throw new ArgumentNullException("acceptValue");
            acceptValue = acceptValue.Trim();
            if (acceptValue.Length == 0)
                throw new ArgumentException("Accept header value cannot be empty", "acceptValue");
            this.acceptValue = acceptValue;
            this.qualityFactor = qualityFactor;
        }

        public static string TryExtractQualityFactor(string acceptValue, out float qualityFactor)
        {
            qualityFactor = 1;
            if (acceptValue.IndexOf(';') > 0)
            {
                var parts = acceptValue.Split(';');
                if (parts.Length == 2)
                {
                    var qualityParts = parts[1].Split('=');
                    if (qualityParts.Length == 2 && qualityParts[0].Trim().ToLowerInvariant() == "q")
                    {
                        float parsedQuality;
                        if (float.TryParse(qualityParts[1].Trim(), out parsedQuality))
                        {
                            qualityFactor = parsedQuality;
                            return parts[0].Trim();
                        }
                    }
                }
            }
            return acceptValue.Trim();
        }

        public override string ToString ()
        {
            return qualityFactor == 1 ? acceptValue
                    : string.Format(CultureInfo.InvariantCulture, "{0};q={1}", acceptValue, qualityFactor);
        }

        public string Value
        {
            get { return acceptValue; }
        }

        public float QualityFactor
        {
            get { return qualityFactor; }
        }
    }
}

