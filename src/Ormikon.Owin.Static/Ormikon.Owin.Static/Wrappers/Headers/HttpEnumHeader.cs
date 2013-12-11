using System;
using System.Collections.Generic;
using System.Linq;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpEnumHeader : HttpStringHeader
    {
        private const string SplitString = ",";
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

        protected void SetEnumValues(ICollection<string> values)
        {
            SetSingleValue(values == null || values.Count == 0 ? null : string.Join(SplitString, values));
        }

        public void AddEnumValue(string enumValue)
        {
            if (string.IsNullOrEmpty(enumValue))
                return;
            var values = new List<string>(GetEnumValues());
            values.Add(enumValue);
            SetEnumValues(values);
        }

        public void RemoveEnumValue(string enumValue)
        {
            SetEnumValues(GetEnumValues().Except(new[] { enumValue }, StringComparer.OrdinalIgnoreCase).ToList());
        }

        public string[] EnumValues
        {
            get { return GetEnumValues(); }
            set { SetEnumValues(value); }
        }
    }
}

