using System;

namespace Ormikon.Owin.Static
{
    /// <summary>
    /// Configuration stettings for StaticMiddleware
    /// </summary>
    public class StaticSettings
    {
        private static char[] sourceSeparators = new [] { ';' };

        public StaticSettings()
        {
            Expires = DateTimeOffset.MinValue;
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
        /// The collection of the source paths
        /// </summary>
        public string[] Sources { get; set; }

        /// <summary>
        /// If files in the collection should be cached
        /// </summary>
        public bool Cached { get; set; }

        /// <summary>
        /// Expires header value for the static content
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// The file pattern for the files that should be included into the collection
        /// </summary>
        public string Include { get; set; }

        /// <summary>
        /// The file pattern for the files that should be excluded from the collection
        /// </summary>
        public string Exclude { get; set; }
    }
}
