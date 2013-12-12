namespace Ormikon.Owin.Static
{
    internal class Location
    {
        private readonly string path;
        private readonly string pathBase;
        private readonly string fullPath;

        public Location(string path, string pathBase)
        {
            this.path = path ?? "";
            this.pathBase = pathBase ?? "";
            fullPath = this.pathBase + this.path;
        }

        public override string ToString ()
        {
            return fullPath;
        }

        public string Path
        {
            get { return path; }
        }

        public string PathBase
        {
            get { return pathBase; }
        }

        public string FullPath
        {
            get { return fullPath; }
        }
    }
}
