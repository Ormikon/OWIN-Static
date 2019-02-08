using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal class HttpPropertyHeader : HttpEnumHeader
    {
        public HttpPropertyHeader(IDictionary<string, StringValues> headers, string code)
            : base(headers, code)
        {
        }

        protected HttpPropertyHeaderValue[] GetPropertyValues()
        {
            return GetEnumValues().Select(v => new HttpPropertyHeaderValue(v)).ToArray();
        }

        protected void SetPropertyValues(ICollection<HttpPropertyHeaderValue> values)
        {
            SetEnumValues(values?.Select(pv => pv.ToString()).ToList());
        }

        public void AddPropertyValue(string enumValue, string propKey, string propValue)
        {
            if (string.IsNullOrEmpty(enumValue) || string.IsNullOrEmpty(propKey))
                return;
            var values = new List<HttpPropertyHeaderValue>(GetPropertyValues());
            var phv = values.FirstOrDefault(val => string.Compare(enumValue, val.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (phv == null)
            {
                phv = new HttpPropertyHeaderValue(enumValue);
                values.Add(phv);
            }
            phv[propKey] = propValue;
            SetPropertyValues(values);
        }

        public void RemovePropertyValue(string enumValue, string propKey)
        {
            if (string.IsNullOrEmpty(enumValue))
                return;

            if (string.IsNullOrEmpty(propKey))
            {
                RemovePropertyValue(enumValue);
                return;
            }

            var values = new List<HttpPropertyHeaderValue>(GetPropertyValues());
            var phv = values.FirstOrDefault(val => string.Compare(enumValue, val.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (phv != null)
            {
                phv[propKey] = null;
                SetPropertyValues(values);
            }
        }

        public void RemovePropertyValue(string enumValue)
        {
            if (string.IsNullOrEmpty(enumValue))
                return;

            var values = new List<HttpPropertyHeaderValue>(GetPropertyValues());
            for (int i = 0; i < values.Count; i++)
            {
                if (string.Compare(enumValue, values[i].Value, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    values.RemoveAt(i);
                    SetPropertyValues(values);
                    break;
                }
            }
        }

        public HttpPropertyHeaderValue[] PropertyValues
        {
            get => GetPropertyValues();
            set => SetPropertyValues(value);
        }
    }

    internal class HttpPropertyHeaderValue
    {
        public HttpPropertyHeaderValue(string enumValue)
        {
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Value = ParseEnumValue(enumValue, Properties);
        }

        private static string ParseEnumValue(string enumValue, IDictionary<string, string> properties)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));
            if (enumValue.Length == 0)
                throw new ArgumentException("Property value cannot be empty.", nameof(enumValue));
            if (enumValue.IndexOf(';') < 0)
                return enumValue;
            var parts = enumValue.Split(';');
            if (parts.Length > 1)
            {
                enumValue = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var part = parts[i];
                    int eqIndex = part.IndexOf('=');
                    string key, val;
                    if (eqIndex > 0)
                    {
                        key = part.Substring(0, eqIndex);
                        val = part.Substring(eqIndex + 1);
                    }
                    else
                    {
                        key = part;
                        val = "";
                    }
                    properties.Add(key.Trim(), val.Trim());
                }
            }
            return enumValue.Trim();
        }

        public override string ToString()
        {
            if (Properties.Count == 0)
                return Value;
            var b = new StringBuilder(Value);
            foreach (var (key, value) in Properties)
            {
                b.Append(';');
                b.Append(key);
                b.Append('=');
                b.Append(value ?? "");
            }

            return b.ToString();
        }

        public string Value { get; }

        public string this[string propName]
        {
            get
            {
                if (Properties.TryGetValue(propName, out var result))
                    return result;
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Properties.ContainsKey(propName))
                        Properties.Remove(propName);
                }
                Properties[propName] = value;
            }
        }

        public IDictionary<string, string> Properties { get; }
    }
}
