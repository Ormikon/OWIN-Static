using System;

namespace Ormikon.AspNetCore.Static
{
    /// <inheritdoc />
    /// <summary>
    /// Configuration settings for StaticMiddleware
    /// </summary>
    public class StaticSettings : StaticSettingsBase
    {
        private static readonly char[] sourceSeparators = { ';' };

        /// <summary>
        /// Creates a new instance with defaults
        /// </summary>
        public StaticSettings()
        {
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
        /// Is hidden files and directories allowed
        /// </summary>
        public bool AllowHidden { get; set; }
    }
}
