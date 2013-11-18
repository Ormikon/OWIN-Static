OWIN Static
===========

OWIN Static library is a library for OWIN application. It makes simple to add static content like scripts and styles to your application.

# Get it on NuGet!
    Install-Package Ormikon.Owin.Static
    
# Documentation

# Basic usage

```C#
using Ormikon.Owin.Static;
using Owin;
using System;

namespace StaticExample
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseStatic() // Loading settings from app.config
                .UseStatic("..\\..\\Index.html") // single file
                .MapStatic("/scripts", new StaticSettings("..\\..\\Scripts") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true, Include = "*.min.*.js" })
                .MapStatic("/styles", new StaticSettings("..\\..\\Styles") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true })
                .MapStatic("/content", new StaticSettings("..\\..\\Styles") { Expires = DateTimeOffset.Now.AddDays(1), Exclude = "*.bmp" })
                .UseWelcomePage("/welcome")
                .UseErrorPage();
        }
    }
}
```

# Map vs Use

MapStatic allows to point root path for the middleware, UseStatic uses root path for this purpose.

# Sources

Static middleware looking for files in these paths and returns the first found file or call the next middleware if file were not found.

In the methods with single string sources parameter possible to add multiple sources separated by comma:

```C#
.UseStatic("Index.html;Error.html;Help.html");
```

# Cache
 
Used only memory cache. If caching is enabled files will be preloaded into memory only once then memory cache uses.

# Expires

Setting up Expires HTTP header for the response.

# Include & Exclude

If include is configured, the files will be filtered by the pointed pattern and will be excluded from included if the excluded is set.
Supports \* (\*, \*\*, \*.\*) pattern style (Forgot to add question mark, it will be added in the future).

# app.config

The middleware suppors loading values from the application configuration file:
```C#
.UseStatic()
```

## Example
 
```xml
<configSections>
    <section name="owinStatic" type="Ormikon.Owin.Static.Config.Section, Ormikon.Owin.Static"/>
</configSections>
<owinStatic>
    <maps>
        <map sources="..\..\Index.html" />
        <map path="/scripts" sources="..\..\Scripts" exclude="**\*1.6.4.js" />
    </maps>
</owinStatic>
```
 
app.config supports the same attributes as StaticSettings. Only source attribute is required all other are optional.
