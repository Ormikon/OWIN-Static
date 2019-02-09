AspNetCore Static
=================

This is port of OWIN Static library to ASP.Net Core. It makes simple to add static content like scripts and styles to your application.

Get it on NuGet
---------------

    (Obsolete) Install-Package Ormikon.Owin.Static
    Install-Package Ormikon.AspNetCore.Static

Documentation
-------------

Basic usage
-----------

```C#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Static.Example
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Optional. Loads settings from the configuration
            services.ConfigureStatic(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Loading settings from the configuration or from wwwroot folder with defaults if no configuration present
            app.UseStatic();
        }
    }
}
```

```C#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Static.Example
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Optional. Loads settings from the configuration
            services.ConfigureStatic(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Loading settings from the configuration or from wwwroot folder with defaults if no configuration present and maps to 'content' path
            app.MapStatic("/content");
        }
    }
}
```

```C#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ormikon.AspNetCore.Static.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use only index.html and map it to a root path with defaults
            app.UseStatic("index.html");
        }
    }
}
```

```C#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ormikon.AspNetCore.Static.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // More complex configuration for files in a several locations (wwwroot is a default folder for sources)
            app.UseStatic("index.html")
                .MapStatic("/scripts", new StaticSettings("scripts") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true, Include = "*.min.*.js" })
                .MapStatic("/styles", new StaticSettings("styles") { Expires = DateTimeOffset.Now.AddDays(1), Cached = true })
                .MapStatic("/content", new StaticSettings("styles") { Expires = DateTimeOffset.Now.AddDays(1), Exclude = "*.bmp" });
        }
    }
}
```

Map vs Use
----------

MapStatic allows to point root path for the middleware, UseStatic uses root path for this purpose.

Sources
-------

Static middleware looking for files in these paths and returns the first found file or call the next middleware if file were not found.

In the methods with single string sources parameter possible to add multiple sources separated by comma:

```C#
.UseStatic("Index.html;Error.html;Help.html");
```

Index file
----------

The middleware automatically searches for the index file if directory was pointed. By default the file name is 'Index.html' but you can change it via configuration settings.

```C#
.UseStatic(new StaticSettings("styles") { DefaultFile = "start.htm" });
```

If you do not want to use the index file just set DefaultFile as null or empty string.

Hidden files
------------

Hidden files and directories are not used by default. They can be enabled by AllowHidden option in StaticSettings.

Redirect if folder
------------------

If a user pointed a folder in the URL like `http://somesite.su/css` this request by default will be redirected to the folder: `http://somesite.su/css/`

It can be disabled via RedirectIfFolderFound setting.

Cache
-----

By default Microsoft MemoryCache is used. Default cache can be configured for the application via DefaultCache static property of StaticSettings or locally for every map via Cache property in StaticSettings. Cache can be any kind of the cache based on the System.Runtime.Caching.ObjectCache class.

Expiration time for the cache uses values from MaxAge if set and then from Expires if set. If both options are not set the cache value will never expire.

Expires
-------

Setting up Expires HTTP header for the response.

MaxAge
------

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
settings.MaxAgeExpression = "60"; //by default time in seconds
```

This property is default for app.config maxAge attribute of a map element.

Include & Exclude
-----------------

If include is configured, the files will be filtered by the pointed pattern and will be excluded from included if the excluded is set.
Supports \* (\*, \*\*, \*.\*), ? pattern style.

appsettings.json
----------------

The middleware supports loading values from the application configuration file:

```C#
services.ConfigureStatic(Configuration);
// or
services.ConfigureStatic(Configuration.GetSection("StaticConfig"));
//...
app.UseStatic();
```

```C#
services.ConfigureStatic(Configuration);
// or
services.ConfigureStatic(Configuration.GetSection("StaticConfig"));
//...
.MapStatic("/content")
```

Configuration Example
-------

```json
{
    "OrmikonStatic" : {
        "Maps" : [
            { "Path": "/scripts", "Sources": "Scripts", "Cached": false, "Expires": "2025-01-01", "Include": "*.js", "Exclude": "**\\*1.6.4.js" },
            { "Path": "/styles", "Sources": "Styles", "Cached": false, "Expires": "2025-01-01", "Include": "*.css", "Exclude": "**\\*debug.css" },
            { "Path": "/home", "Sources": "Index.html" }
        ]
    }
}
```

appsettings.json supports the same attributes as StaticSettings. Only source attribute is required all other are optional.
