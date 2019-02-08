using System;
using Ormikon.AspNetCore.Static.Cache;
using Ormikon.AspNetCore.Static.Extensions;

namespace Ormikon.AspNetCore.Static
{
    /// <summary>
    /// Configuration settings for StaticMiddleware
    /// </summary>
    public class StaticSettings
    {
        public const string DefaultFileValue = "index.html;index.htm;start.html;start.htm;default.html;default.htm";
        public const string DefaultCompressedTypesFilter = "text/*;*/xml;application/*javascript;application/*json*"
            + ";application/*+xml;image/*+xml";
        private static readonly char[] sourceSeparators = { ';' };

        public static readonly IStaticCache DefaultMemoryCache = new SimpleMemoryStaticCache();
        private static IStaticCache defaultCache = DefaultMemoryCache;

        private int maxAge;

        public StaticSettings()
        {
            Expires = DateTimeOffset.MinValue;
            RedirectIfFolderFound = true;
            DefaultFile = DefaultFileValue;
            CompressedContentTypes = DefaultCompressedTypesFilter;
        }

        /// <summary>
        /// Creates a new instance of StaticSettings
        /// </summary>
        /// <param name="sources">A collection of the source paths</param>
        public StaticSettings(params string[] sources) :
            this()
        {
            Sources = sources;
        }

        /// <summary>
        /// Creates a new instance of StaticSettings
        /// </summary>
        /// <param name="sources">Single source path or a collection separated by comma</param>
        public StaticSettings(string sources) :
            this()
        {
            if (sources != null && sources.IndexOfAny(sourceSeparators) >= 0)
                Sources = sources.Split(sourceSeparators, StringSplitOptions.RemoveEmptyEntries);
            else
                Sources = new []{sources};
        }

        /// <summary>
        /// Default memory cache that will used in the middleware if custom cache was not set.
        /// </summary>
        public static IStaticCache DefaultCache
        {
            get => defaultCache;
            set => defaultCache = value ?? DefaultMemoryCache;
        }

        /// <summary>
        /// The collection of the source paths
        /// </summary>
        public string[] Sources { get; set; }

        /// <summary>
        /// If files in the collection should be cached
        /// </summary>
        public bool Cached { get; set; }

        /// <summary>
        /// Cache implementation. In Memory Cache will be used by default
        /// </summary>
        public IStaticCache Cache { get; set; }

        /// <summary>
        /// Expires header value for the static content
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// Max age value for the static content in seconds
        /// </summary>
        public int MaxAge
        {
            get => maxAge;
            set => maxAge = value < 0 ? 0 : value;
        }

        /// <summary>
        /// Max age value for the static content as expression. Supported numbers (one-twelve | 0-...) and periods
        /// (second(s) | sec(s) | minute(s) | min(s) | hour(s) | day(s) | week(s) | month(s) | year(s))
        /// <example>
        /// 1Day
        /// </example>
        /// <example>
        /// oneDay
        /// </example>
        /// <example>
        /// threeYearsTwoweeksFiveDays
        /// </example>
        /// <example>
        /// Year5Days
        /// </example>
        /// </summary>
        public string MaxAgeExpression
        {
            set { maxAge = value.ToMaxAge(); }
        }

        /// <summary>
        /// Default file in the directory. index.html by default
        /// </summary>
        public string DefaultFile { get; set; }

        /// <summary>
        /// Send redirect request if user requested folder without slash: http://someFolder -> http://someFolder/.
        /// Is true by default
        /// </summary>
        public bool RedirectIfFolderFound { get; set; }

        /// <summary>
        /// The file pattern for the files that should be included into the collection
        /// </summary>
        public string Include { get; set; }

        /// <summary>
        /// The file pattern for the files that should be excluded from the collection
        /// </summary>
        public string Exclude { get; set; }

        /// <summary>
        /// The pattern for content types that can be compressed in the response
        /// </summary>
        /// <value>The compressed content types in filter like representation.</value>
        public string CompressedContentTypes { get; set; }

        /// <summary>
        /// Is hidden files and directories allowed
        /// </summary>
        public bool AllowHidden { get; set; }
    }
}
