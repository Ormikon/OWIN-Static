using System;
using System.Collections.Generic;
using System.Collections;

namespace Ormikon.Owin.Static.Headers
{
    internal abstract class HttpHeaders : IHttpHeaders
    {
        private readonly IDictionary<string, string[]> headers;

        protected HttpHeaders ()
            : this (new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase))
        {
        }

        protected HttpHeaders(IDictionary<string, string[]> headers)
        {
            this.headers = headers;
        }

        #region abstract members

        protected abstract IHttpHeaders CreateInstance();

        #endregion

        #region IHttpHeaders implementation

        public void CopyTo(IDictionary<string, string[]> headers)
        {
            foreach(var kv in this.headers)
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

        public void Add(string key, string[] value)
        {
            headers.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return headers.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return headers.Remove(key);
        }

        public bool TryGetValue(string key, out string[] value)
        {
            return headers.TryGetValue(key, out value);
        }

        public string[] this[string index]
        {
            get
            {
                return headers[index];
            }
            set
            {
                headers[index] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return headers.Keys;
            }
        }

        public ICollection<string[]> Values
        {
            get
            {
                return headers.Values;
            }
        }

        #endregion

        #region ICollection implementation

        public void Add(KeyValuePair<string, string[]> item)
        {
            headers.Add(item);
        }

        public void Clear()
        {
            headers.Clear();
        }

        public bool Contains(KeyValuePair<string, string[]> item)
        {
            return headers.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
        {
            headers.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string[]> item)
        {
            return headers.Remove(item);
        }

        public int Count
        {
            get
            {
                return headers.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return headers.IsReadOnly;
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
        {
            return headers.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)headers).GetEnumerator();
        }

        #endregion
    }
}

