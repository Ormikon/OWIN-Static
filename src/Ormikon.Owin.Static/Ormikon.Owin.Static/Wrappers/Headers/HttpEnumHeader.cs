using System;
using System.Collections.Generic;
using System.Linq;

namespace Ormikon.Owin.Static.Headers
{
    internal class HttpEnumHeader : HttpStringHeader
    {
        protected const string SplitString = ",";
        private static readonly char[] splitChar = new [] { ',' };

        public HttpEnumHeader(IDictionary<string, string[]> headers, string code)
            : base(headers, code)
        {
        }

        protected string[] GetEnumValues()
        {
            string stringValue = GetSingleValue();
            if (string.IsNullOrEmpty(stringValue))
            {
                return new string[0];
            }
            return stringValue.Split(splitChar, StringSplitOptions.RemoveEmptyEntries)
                    .Select(ev => ev.Trim())
                    .ToArray();
        }

        public void AddEnumValue(string enumValue)
        {
            var values = new List<string>(GetEnumValues());
            values.Add(enumValue);
            SetSingleValue(string.Join(SplitString, values));
        }

        public string[] EnumValues
        {
            get { return GetEnumValues(); }
        }
    }
}

