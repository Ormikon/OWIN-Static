using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Microsoft.Owin.Hosting;
using Owin;
using System.Net.Http;
using System.Net;

namespace Ormikon.Owin.Static.Tests
{
    [TestFixture]
    public class MiddlewareBasicTests
    {
        private const string BaseAddress = "http://localhost:8099";
        private const string ContentFolder = "content";
        private IDisposable webApp;

        [TestFixtureSetUp]
        public void SetUp()
        {
            CreateContent();
            try{
            webApp = WebApp.Start(BaseAddress,
                appBuilder =>
                {
                    appBuilder.UseStatic(ContentFolder);
                });
            }
            catch(Exception exception)
            {
                GC.KeepAlive(exception);
            }
        }

        [TestFixtureTearDown]
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
    }
}

