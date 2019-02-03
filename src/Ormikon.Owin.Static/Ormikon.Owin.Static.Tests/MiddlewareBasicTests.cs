using System;
using NUnit.Framework;
using System.IO;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Ormikon.Owin.Static.Tests
{
    [TestFixture]
    public class MiddlewareBasicTests
    {
        private class WebHostTestStartup
        {
            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseStatic()
                    .MapStatic("/nested1")
                    .MapStatic("/nested1/nested2");
            }
        }

        private const string BaseAddress = "http://localhost:8099";
        private const string ContentFolder = "content";
        private IWebHost webApp;

        [OneTimeSetUp]
        public void SetUp()
        {
            CreateContent();
            try
            {
                webApp = WebHost.CreateDefaultBuilder()
                    .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "content"))
                    .UseUrls(BaseAddress)
                    .UseStartup<WebHostTestStartup>()
                    .Build();

                webApp.StartAsync();
            }
            catch (Exception exception)
            {
                GC.KeepAlive(exception);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (webApp != null)
                webApp.Dispose();
            RemoveContent();
        }

        private static void CreateContent()
        {
            RemoveContent();
            string currentDir = Directory.GetCurrentDirectory();
            Directory.CreateDirectory(ContentFolder);
            Directory.SetCurrentDirectory(Path.Combine(currentDir, ContentFolder));

            File.WriteAllText("index.html", "Index file with HTML content type.");
            File.WriteAllText("script.js", "Java script content here");
            File.WriteAllText("style.css", "A couple of css styles");

            Directory.SetCurrentDirectory(currentDir);
        }

        private static void RemoveContent()
        {
            if (Directory.Exists(ContentFolder))
                Directory.Delete(ContentFolder, true);
        }

        [TestCase(BaseAddress)]
        [TestCase(BaseAddress + "/")]
        [TestCase(BaseAddress + "/index.html")]
        public void IndexTest(string indexAddress)
        {
            var client = new HttpClient();
            var rm = client.GetAsync(indexAddress).Result;
            Assert.AreEqual("text/html", rm.Content.Headers.ContentType.MediaType);
            Assert.AreEqual("Index file with HTML content type.", rm.Content.ReadAsStringAsync().Result);
        }

        [TestCase(BaseAddress + "/script.js", "application/javascript", "Java script content here")]
        [TestCase(BaseAddress + "/style.css", "text/css", "A couple of css styles")]
        public void ContentTest(string contentAddress, string contentType, string content)
        {
            var client = new HttpClient();
            var rm = client.GetAsync(contentAddress).Result;
            Assert.AreEqual(contentType, rm.Content.Headers.ContentType.MediaType);
            Assert.AreEqual(content, rm.Content.ReadAsStringAsync().Result);
        }

        [TestCase(BaseAddress + "/nested1/script.js")]
        [TestCase(BaseAddress + "/nested1/nested2/script.js")]
        public void NestedPathsTest(string contentAddress)
        {
            var client = new HttpClient();
            var rm = client.GetAsync(contentAddress).Result;
            Assert.AreEqual(HttpStatusCode.OK, rm.StatusCode, "Unable to get resources from nested paths.");
        }
    }
}
