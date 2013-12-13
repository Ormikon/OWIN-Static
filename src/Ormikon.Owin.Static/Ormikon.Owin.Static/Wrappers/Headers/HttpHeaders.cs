using System;
using System.Collections.Generic;
using System.Collections;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal abstract class HttpHeaders : IHttpHeaders
    {
        private readonly IDictionary<string, string[]> internalHeaders;

        protected HttpHeaders ()
            : this (new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase))
        {
        }

        protected HttpHeaders(IDictionary<string, string[]> internalHeaders)
        {
            this.internalHeaders = internalHeaders;
        }

        #region abstract members

        protected abstract IHttpHeaders CreateInstance();

        #endregion

        #region IHttpHeaders implementation

        public void CopyTo(IDictionary<string, string[]> headers)
        {
            foreach(var kv in internalHeaders)
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

        public bool TryGetValue(string key, out string[] value)
        {
            return internalHeaders.TryGetValue(key, out value);
        }

        public string[] this[string index]
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

        public ICollection<string[]> Values
        {
            get
            {
                return internalHeaders.Values;
            }
        }

        #endregion

        #region ICollection implementation

        public void Add(KeyValuePair<string, string[]> item)
        {
            internalHeaders.Add(item);
        }

        public void Clear()
        {
            internalHeaders.Clear();
        }

        public bool Contains(KeyValuePair<string, string[]> item)
        {
            return internalHeaders.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
        {
            internalHeaders.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string[]> item)
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

        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
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

