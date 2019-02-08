using Microsoft.AspNetCore.Http;

namespace Ormikon.AspNetCore.Static.Wrappers
{
    internal struct Location
    {
        public Location(HttpRequest request) : this(request.Path, request.PathBase)
        {
        }

        public Location(PathString path, PathString pathBase)
        {
            Path = path;
            PathBase = pathBase;
            FullPath = PathBase + Path;
        }

        public void SetToRequest(HttpRequest request)
        {
            request.Path = Path;
            request.PathBase = PathBase;
        }

        public override string ToString ()
        {
            return FullPath;
        }

        public PathString Path { get; }

        public PathString PathBase { get; }

        public PathString FullPath { get; }
    }
}
