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
                .UseWelcomePage("/welcome")
                .UseErrorPage();
        }
    }
}
```

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
            app.MapStatic("/content") // Loading settings from app.config and map to 'content' path
                .UseWelcomePage("/welcome")
                .UseErrorPage();
        }
    }
}
```

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
            app.UseStatic("..\\..\\Index.html")
                .MapStatic("/content") // Loading settings from app.config and map to 'content' path
                .UseWelcomePage("/welcome")
                .UseErrorPage();
        }
    }
}
```

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

# Index file

The middleware automatically seaches for the index file if directory was pointed. By default the file name is 'Index.html' but you can change it via configuration settings.

```C#
.UseStatic(new StaticSettings("..\\..\\Styles") { DefaultFile = "start.htm" });
```

If you do not want to use the index file just set DefaultFile as null or empty string.

# Hidden files

Hidden files and directories are not used by default. They can be enabled by AllowHidden option in StaticSettings.

# Redirect if folder

If a user pointed a folder in the URL like http://somesite.su/css this request by default will be redirected to the folder: http://somesite.su/css/

It can be disabled via RedirectIfFolderFound setting.

# Cache
 
By default Microsoft MemoryCache is used. Default cache can be configured for the application via DefaultCache static property of StaticSettings or localy for every map via Cache property in StaticSettings. Cache can be any kind of the cache based on the System.Runtime.Caching.ObjectCache class.

Expiration time for the cache uses values from MaxAge if set and then from Expires if set. If both options are not set the cache value will never expire.

# Expires

Setting up Expires HTTP header for the response.

# MaxAge

Another way to set content lifetime is MaxAge. This setting adds max-age value for 'Cache-Control' response header.
StaticSettings has MaxAgeExpression property to assign max-age in a user friendly way:

```C#
settings.MaxAgeExpression = "1day";
```
```C#
settings.MaxAgeExpression = "5days";
```
```C#
settings.MaxAgeExpression = "oneDayTwoWeeksFiveYears55sec";
```
```C#
settings.MaxAgeExpression = "60";//by default time in seconds
```

This property is default for app.config maxAge attribute of a map element.

# Include & Exclude

If include is configured, the files will be filtered by the pointed pattern and will be excluded from included if the excluded is set.
Supports \* (\*, \*\*, \*.\*), ? pattern style.

# app.config

The middleware suppors loading values from the application configuration file:
```C#
.UseStatic()
```
```C#
.MapStatic("/content")
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
