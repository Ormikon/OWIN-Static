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
        private static readonly Regex slashes = new Regex(@"[\\,\/]+", RegexOptions.Compiled);
        private static readonly Regex multyStars1 = new Regex(@"(?<=\/|^)\*{3,}(?=\/|$)", RegexOptions.Compiled);
        private static readonly Regex multyStars2 = new Regex(@"((?<!\/)\*{2,})|(\*{2,}(?!\/|$))", RegexOptions.Compiled);
        private static readonly Regex searchGroups = new Regex(@"(\/\*\*\/?)|(\*)|(\?)|(\.)", RegexOptions.Compiled);

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
                if (i > 0)
                    rb.Append('|');
                ConvertFilter(fArray[i], rb);
            }
            rb.Append('$');

            return new Regex(rb.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        private void ConvertFilter(string filter, StringBuilder regexBuilder)
        {
            filter = ThrowIfInvalid(filter);
            filter = OptimizeFilter(filter);
            regexBuilder.Append(@"((.*?\/)?");// group start
            filter = searchGroups.Replace(filter,
                                          match =>
                                              {
                                                  if (match.Groups[1].Success)
                                                      return @"((\/.*?\/)|\/)";
                                                  if (match.Groups[2].Success)
                                                      return @"[^\/]*?";
                                                  if (match.Groups[3].Success)
                                                      return ".";
                                                  return @"\.";
                                              });
            regexBuilder.Append(filter);
            regexBuilder.Append(")");// group end
        }

        private static string OptimizeFilter(string filter)
        {
            filter = slashes.Replace(filter, "/").Trim('/');
            filter = multyStars1.Replace(filter, "**");
            return multyStars2.Replace(filter, "*");
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
