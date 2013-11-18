using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ormikon.Owin.Static
{
    internal class FileFilter
    {
        private static readonly char[] filterSeparator = new[] { ';' };

        private readonly Regex filterRegEx;
        private readonly char[] invalidChars;

        public FileFilter(string filters)
        {
            invalidChars = Path.GetInvalidPathChars().Except(new[] { '*' }).ToArray();
            filterRegEx = Convert(filters);
        }

        private string ThrowIfInvalid(string filter)
        {
            if (filter.IndexOfAny(invalidChars) >= 0)
            {
                throw new ArgumentException("The filter contains invalid chars: " + filter, "filter");
            }
            return filter;
        }

        private Regex Convert(string filters)
        {
            if (filters == null)
                return null;
            var fArray = filters
                .Split(filterSeparator, StringSplitOptions.RemoveEmptyEntries)
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
                string f = ThrowIfInvalid(fArray[i]);
                if (i > 0)
                    rb.Append('|');
                rb.Append("(.*?");
                f = f.Replace("\\", "\\/").Replace(".", "\\.")
                    .Replace("?", ".")
                    .Replace("/**/", "-#####-")
                    .Replace("**", "-######-")
                    .Replace("*", "[^\\/]*?")
                    .Replace("-#####-", "\\/.*?")
                    .Replace("-######-", ".*?");
                rb.Append(f);
                rb.Append(')');
            }
            rb.Append('$');

            return new Regex(rb.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool Contains(string fileName)
        {
            if (filterRegEx == null)
                return false;
            // to linux
            fileName = fileName.Replace("\\", "/");
            return filterRegEx.IsMatch(fileName);
        }

        public bool IsActive()
        {
            return filterRegEx != null;
        }
    }
}
