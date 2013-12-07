using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

namespace Ormikon.Owin.Static.Filters
{
    internal abstract class FilterBase : IFilter
    {
        private static readonly char[] filterSeparator = new[] { ';' };

        private readonly Regex filterRegEx;

        protected FilterBase(string filters)
        {
            filterRegEx = Convert(filters);
        }

        private Regex Convert(string filters)
        {
            if (filters == null)
                return null;
            var fArray = filters
                .Split(FilterSeperators, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim())
                .Where(f => f.Length > 0)
                .ToArray();
            if (fArray.Length == 0)
            {
                return null;
            }
            var rb = new StringBuilder("^");
            for (int i = 0; i < fArray.Length; i++)
            {
                if (i > 0)
                    rb.Append('|');
                ConvertFilter(fArray[i], rb);
            }
            rb.Append('$');

            return new Regex(rb.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        protected abstract void ConvertFilter(string filter, StringBuilder regexBuilder);

        #region IFilter implementation

        public virtual bool Contains (string test)
        {
            if (filterRegEx == null)
                return false;
            return filterRegEx.IsMatch(test);
        }

        public bool IsActive ()
        {
            return filterRegEx != null;
        }

        #endregion

        protected virtual char[] FilterSeperators
        {
            get{ return filterSeparator; }
        }
    }
}

