using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Ormikon.Owin.Static.Filters
{
    internal class ContentTypeFilter : FilterBase
    {
        private static readonly Regex multyStars = new Regex(@"\*{2,}", RegexOptions.Compiled);
        private static readonly Regex searchGroups = new Regex(@"(\/)|(\*)|(\?)|(\.)|(\+)", RegexOptions.Compiled);

        public ContentTypeFilter(string filters)
            : base(filters)
        {
        }

        private static string OptimizeFilter(string filter)
        {
            return multyStars.Replace(filter, "*");
        }

        #region implemented abstract members of FilterBase

        protected override void ConvertFilter (string filter, StringBuilder regexBuilder)
        {
            filter = OptimizeFilter(filter);
            regexBuilder.Append("(");
            filter = searchGroups.Replace(filter,
                                          match =>
                                              {
                                                  if (match.Groups[1].Success)
                                                      return @"\/";
                                                  if (match.Groups[2].Success)
                                                      return @".*";
                                                  if (match.Groups[3].Success)
                                                      return ".";
                                                  if (match.Groups[4].Success)
                                                      return @"\.";
                                                  return @"\+";
                                              });
            regexBuilder.Append(filter);
            regexBuilder.Append("(;.*){0,1})");
        }

        #endregion
    }
}

