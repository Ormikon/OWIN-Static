using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpEnumHeader : HttpStringHeader
    {
        private const char SplitChar = ',';

        public HttpEnumHeader(IDictionary<string, StringValues> headers, string code)
            : base(headers, code)
        {
        }

        private static void SplitValues(string value, ICollection<string> enumValues)
        {
            int startIndex = 0;
            int separatorIdx;
            while ((separatorIdx = value.IndexOf(SplitChar, startIndex)) >= 0)
            {
                if (startIndex != separatorIdx)
                    enumValues.Add(value.Substring(startIndex, separatorIdx - startIndex));
                startIndex = separatorIdx + 1;
            }
            string split = startIndex == 0 ? value : value.Substring(startIndex);
            if (split.Length > 0)
                enumValues.Add(split);
        }

        private List<string> GetEnumValuesList()
        {
            var values = Values;
            if (values.Count == 0)
                return new List<string>(0);
            var enumValues = new List<string>();
            for (int i = 0; i < values.Count; i++)
            {
                SplitValues(values[i], enumValues);
            }

            return enumValues;
        }

        protected string[] GetEnumValues()
        {
            return GetEnumValuesList().ToArray();
        }

        protected void SetEnumValues(ICollection<string> values)
        {
            SetSingleValue(values == null || values.Count == 0 ? null : string.Join(SplitString, values));
        }

        public void AddEnumValue(string enumValue)
        {
            if (string.IsNullOrEmpty(enumValue))
                return;
            var values = GetEnumValuesList();
            values.Add(enumValue);
            SetEnumValues(values);
        }

        public void RemoveEnumValue(string enumValue)
        {
            if (string.IsNullOrEmpty(enumValue))
                return;
            var values = GetEnumValuesList();
            if (values.Remove(enumValue))
                SetEnumValues(values);
        }

        public string[] EnumValues
        {
            get { return GetEnumValues(); }
            set { SetEnumValues(value); }
        }
    }
}

