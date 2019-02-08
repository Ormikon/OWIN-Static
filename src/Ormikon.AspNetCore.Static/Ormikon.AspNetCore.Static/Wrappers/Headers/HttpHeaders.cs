using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Ormikon.AspNetCore.Static.Wrappers.Headers
{
    internal abstract class HttpHeaders : IHttpHeaders
    {
        private readonly IDictionary<string, StringValues> internalHeaders;

        protected HttpHeaders ()
            : this (new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase))
        {
        }

        protected HttpHeaders(IDictionary<string, StringValues> internalHeaders)
        {
            this.internalHeaders = internalHeaders;
        }

        #region abstract members

        protected abstract IHttpHeaders CreateInstance();

        #endregion

        #region IHttpHeaders implementation

        public void CopyTo(IDictionary<string, StringValues> headers, params string[] except)
        {
            var copiedHeaders = except == null || except.Length == 0
                                    ? internalHeaders
                                    : internalHeaders.Where(
                                        h => !except.Contains(h.Key, StringComparer.OrdinalIgnoreCase));
            foreach(var kv in copiedHeaders)
            {
                headers[kv.Key] = kv.Value;
            }
        }

        public IHttpHeaders Clone()
        {
            var result = CreateInstance();
            CopyTo(result);
            return result;
        }

        #endregion

        #region ICloneable implementation

        object ICloneable.Clone ()
        {
            return Clone();
        }

        #endregion

        #region IDictionary implementation

        public void Add(string key, StringValues value)
        {
            internalHeaders.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return internalHeaders.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return internalHeaders.Remove(key);
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            return internalHeaders.TryGetValue(key, out value);
        }

        public StringValues this[string index]
        {
            get
            {
                return internalHeaders[index];
            }
            set
            {
                internalHeaders[index] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return internalHeaders.Keys;
            }
        }

        public ICollection<StringValues> Values
        {
            get
            {
                return internalHeaders.Values;
            }
        }

        #endregion

        #region ICollection implementation

        public void Add(KeyValuePair<string, StringValues> item)
        {
            internalHeaders.Add(item);
        }

        public void Clear()
        {
            internalHeaders.Clear();
        }

        public bool Contains(KeyValuePair<string, StringValues> item)
        {
            return internalHeaders.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex)
        {
            internalHeaders.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            return internalHeaders.Remove(item);
        }

        public int Count
        {
            get
            {
                return internalHeaders.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return internalHeaders.IsReadOnly;
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            return internalHeaders.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)internalHeaders).GetEnumerator();
        }

        #endregion
    }
}

