using Owin;

namespace Ormikon.Owin.Static.Example
{
    internal class Startup
    {
        private string ContentPath(string path)
        {
            return "..\\..\\Content\\" + path;
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.MapStatic("/content")
                      //.UseStatic(ContentPath("Index.html"))
                      //.MapStatic("/css", ContentPath("css"))
                      //.MapStatic("/js", ContentPath("js"))
                      //.MapStatic("/font-awesome", ContentPath("font-awesome"))
                      .UseStatic(new StaticSettings("..\\..\\Content\\"){Cached = true})
                      .UseErrorPage();
        }
    }
}
