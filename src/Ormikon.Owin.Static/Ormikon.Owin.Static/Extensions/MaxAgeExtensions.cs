using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormikon.Owin.Static.Extensions
{
    internal static class MaxAgeExtensions
    {
        private enum Periods
        {
            Year,
            Month,
            Week,
            Day,
            Hour,
            Minute,
            Second
        }

        private static readonly IDictionary<string, Periods> periodTokens =
            new Dictionary<string, Periods>(StringComparer.Ordinal)
                {
                    {"years", Periods.Year},
                    {"year", Periods.Year},
                    {"y", Periods.Year},
                    {"months", Periods.Month},
                    {"month", Periods.Month},
                    {"mths", Periods.Month},
                    {"mth", Periods.Month},
                    {"weeks", Periods.Week},
                    {"week", Periods.Week},
                    {"w", Periods.Week},
                    {"days", Periods.Day},
                    {"day", Periods.Day},
                    {"d", Periods.Day},
                    {"hours", Periods.Hour},
                    {"hour", Periods.Hour},
                    {"h", Periods.Hour},
                    {"minutes", Periods.Minute},
                    {"minute", Periods.Minute},
                    {"mins", Periods.Minute},
                    {"min", Periods.Minute},
                    {"seconds", Periods.Second},
                    {"second", Periods.Second},
                    {"secs", Periods.Second},
                    {"sec", Periods.Second},
                    {"s", Periods.Second}
                };

        private static readonly IDictionary<string, uint> numberTokens =
            new Dictionary<string, uint>(StringComparer.Ordinal)
                {
                    {"one", 1},
                    {"two", 2},
                    {"three", 2},
                    {"four", 4},
                    {"five", 5},
                    {"six", 6},
                    {"seven", 7},
                    {"eight", 8},
                    {"nine", 9},
                    {"ten", 10},
                    {"eleven", 11},
                    {"twelve", 12}
                };

        private static readonly string[] tokens = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight",
            "nine", "ten", "eleven", "twelve", "years", "year", "y", "months", "month", "mths", "mth", "weeks", "week", "w",
            "days", "day", "d", "hours", "hour", "h", "minutes", "minute", "mins", "min", "seconds", "second", "secs", "sec", "s" };

        private static string GetDecimal(string token)
        {
            if (string.IsNullOrEmpty(token))
                return "";
            var sb = new StringBuilder();
            foreach (char ch in token)
            {
                if (char.IsNumber(ch))
                    sb.Append(ch);
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }

        public static int ToMaxAge(this string maxAgeExpression)
        {
            if (string.IsNullOrWhiteSpace(maxAgeExpression))
                return 0;
            maxAgeExpression = maxAgeExpression.Trim();
            int maxAge;
            if (int.TryParse(maxAgeExpression, out maxAge))
                return maxAge < 0 ? 0 : maxAge;
            maxAgeExpression = maxAgeExpression.Replace(" ", "").Replace("\t", "").ToLowerInvariant();
            bool hasNumbersBefore = false;
            uint number = 1;
            while (maxAgeExpression.Length > 0)
            {
                string token = tokens.FirstOrDefault(t => maxAgeExpression.StartsWith(t, StringComparison.Ordinal));
                if (token != null)
                {
                    if (!hasNumbersBefore)
                    {
                        if (numberTokens.TryGetValue(token, out number))
                        {
                            hasNumbersBefore = true;
                            maxAgeExpression = maxAgeExpression.Remove(0, token.Length);
                            continue;
                        }
                    }
                    if (token.Length > 1 && token.EndsWith("s") && !hasNumbersBefore)
                        token.Remove(token.Length - 1);
                    Periods period;
                    if (!periodTokens.TryGetValue(token, out period))
                    {
                        throw new ArgumentException("Inavlid max-age token value: '" + token + "'", "maxAgeExpression");
                    }
                    maxAgeExpression = maxAgeExpression.Remove(0, token.Length);
                    int periodSeconds = 1;
                    switch (period)
                    {
                        case Periods.Year:
                            periodSeconds = 365*24*60*60;
                            break;
                        case Periods.Month:
                            periodSeconds = 30*24*60*60;
                            break;
                        case Periods.Week:
                            periodSeconds = 7*24*60*60;
                            break;
                        case Periods.Day:
                            periodSeconds = 24*60*60;
                            break;
                        case Periods.Hour:
                            periodSeconds = 60*60;
                            break;
                        case Periods.Minute:
                            periodSeconds = 60;
                            break;
                        case Periods.Second:
                            periodSeconds = 1;
                            break;
                    }
                    if (hasNumbersBefore)
                        periodSeconds *= (int)number;
                    hasNumbersBefore = false;
                    maxAge += periodSeconds;
                }
                else
                {
                    if (!hasNumbersBefore)
                    {
                        string dec = GetDecimal(maxAgeExpression);
                        if (uint.TryParse(dec, out number))
                        {
                            hasNumbersBefore = true;
                            maxAgeExpression = maxAgeExpression.Remove(0, dec.Length);
                            continue;
                        }
                    }
                    throw new ArgumentException("Inavlid max-age token value: '" + maxAgeExpression + "'", "maxAgeExpression");
                }
            }

            return maxAge;
        }
    }
}
