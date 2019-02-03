using Microsoft.AspNetCore.Http;

namespace Ormikon.Owin.Static.Wrappers
{
    internal struct Location
    {
        private readonly PathString path;
        private readonly PathString pathBase;
        private readonly PathString fullPath;

        public Location(HttpRequest request) : this(request.Path, request.PathBase)
        {
        }

        public Location(PathString path, PathString pathBase)
        {
            this.path = path;
            this.pathBase = pathBase;
            fullPath = this.pathBase + this.path;
        }

        public void SetToRequest(HttpRequest request)
        {
            request.Path = path;
            request.PathBase = pathBase;
        }

        public override string ToString ()
        {
            return fullPath;
        }

        public PathString Path
        {
            get { return path; }
        }

        public PathString PathBase
        {
            get { return pathBase; }
        }

        public PathString FullPath
        {
            get { return fullPath; }
        }
    }
}
