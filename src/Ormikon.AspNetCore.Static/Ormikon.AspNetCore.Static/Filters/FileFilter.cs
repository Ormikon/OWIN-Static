using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ormikon.AspNetCore.Static.Filters
{
    internal class FileFilter : FilterBase
    {
        private static readonly Regex slashes = new Regex(@"[\\,\/]+", RegexOptions.Compiled);
        private static readonly Regex multiStars1 = new Regex(@"(?<=\/|^)\*{3,}(?=\/|$)", RegexOptions.Compiled);
        private static readonly Regex multiStars2 = new Regex(@"((?<!\/)\*{2,})|(\*{2,}(?!\/|$))", RegexOptions.Compiled);
        private static readonly Regex searchGroups = new Regex(@"(\/\*\*\/?)|(\*)|(\?)|(\.)|(\+)|(\^)|(\$)", RegexOptions.Compiled);

        private readonly char[] invalidChars = Path.GetInvalidPathChars().Except(new[] { '*', '?' }).ToArray();

        public FileFilter(string filters)
            : base(filters)
        {
        }

        private string ThrowIfInvalid(string filter)
        {
            if (filter.IndexOfAny(invalidChars) >= 0)
            {
                throw new ArgumentException("The filter contains invalid chars: " + filter, "filter");
            }
            return filter;
        }

        private static string OptimizeFilter(string filter)
        {
            filter = slashes.Replace(filter, "/").Trim('/');
            filter = multiStars1.Replace(filter, "**");
            return multiStars2.Replace(filter, "*");
        }

        protected override void ConvertFilter(string filter, StringBuilder regexBuilder)
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
                                                  if (match.Groups[4].Success)
                                                      return @"\.";
                                                  if (match.Groups[5].Success)
                                                      return @"\+";
                                                  if (match.Groups[6].Success)
                                                      return @"\^";
                                                  return @"\$";
                                              });
            regexBuilder.Append(filter);
            regexBuilder.Append(")");// group end
        }

        public override bool Contains(string fileName)
        {
            if (!IsActive())
                return false;
            // to linux
            fileName = fileName.Replace("\\", "/");
            return base.Contains(fileName);
        }
    }
}
