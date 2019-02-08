using NUnit.Framework;
using Ormikon.AspNetCore.Static.Filters;

namespace Ormikon.AspNetCore.Static.Tests
{
    [TestFixture]
    public class ContentTypeFilterTests
    {
        [Test]
        public void ExactMatch()
        {
            var filter = new ContentTypeFilter("text/html");
            Assert.IsTrue(filter.Contains("text/html"));
            Assert.IsTrue(filter.Contains("text/html; charset=UTF-8"));
            Assert.IsFalse(filter.Contains("text/htm"));
            Assert.IsFalse(filter.Contains("ext/html"));
            Assert.IsFalse(filter.Contains("texthtml"));
            Assert.IsFalse(filter.Contains("text/htmler"));
            Assert.IsFalse(filter.Contains("nontext/html"));
        }

        [Test]
        public void TestQuestionMark()
        {
            var filter = new ContentTypeFilter("t?xt/html");
            Assert.IsTrue(filter.Contains("text/html"));
            Assert.IsTrue(filter.Contains("tuxt/html"));
            Assert.IsFalse(filter.Contains("txt/html"));
            Assert.IsFalse(filter.Contains("mext/html"));
            filter = new ContentTypeFilter("t?xt/ht?l");
            Assert.IsTrue(filter.Contains("text/html"));
            Assert.IsTrue(filter.Contains("tuxt/htpl"));
            Assert.IsFalse(filter.Contains("txt/htl"));
            Assert.IsFalse(filter.Contains("txt/html"));
            Assert.IsFalse(filter.Contains("text/htl"));
            Assert.IsFalse(filter.Contains("mext/htmp"));
        }

        [Test]
        public void TestStar()
        {
            var filter = new ContentTypeFilter("*t/html");
            Assert.IsTrue(filter.Contains("text/html"));
            Assert.IsTrue(filter.Contains("ext/html"));
            Assert.IsTrue(filter.Contains("xt/html"));
            Assert.IsTrue(filter.Contains("t/html"));
            Assert.IsFalse(filter.Contains("texm/htl"));
            Assert.IsFalse(filter.Contains("tex/htl"));
            filter = new ContentTypeFilter("t*x*/ht*l");
            Assert.IsTrue(filter.Contains("text/html"));
            Assert.IsTrue(filter.Contains("tx/html"));
            Assert.IsTrue(filter.Contains("txt/html"));
            Assert.IsTrue(filter.Contains("tex/html"));
            Assert.IsFalse(filter.Contains("text/htmk"));
            Assert.IsFalse(filter.Contains("txt/hkml"));
            filter = new ContentTypeFilter("text/*");
            Assert.IsTrue(filter.Contains("text/html"));
            Assert.IsTrue(filter.Contains("text/html; charset=UTF-8"));
            Assert.IsTrue(filter.Contains("text/html ; charset=UTF-8"));
            Assert.IsTrue(filter.Contains("text/html; charset=UTF/-8"));
        }

        [Test]
        public void TestDefaultFilter()
        {
            var filter = new ContentTypeFilter(StaticSettings.DefaultCompressedTypesFilter);
            Assert.IsTrue(filter.Contains("text/xml"));
            Assert.IsTrue(filter.Contains("text/html"));
            Assert.IsTrue(filter.Contains("text/plain"));
            Assert.IsTrue(filter.Contains("application/xml"));
            Assert.IsTrue(filter.Contains("application/json"));
            Assert.IsFalse(filter.Contains("nontext/nonxml"));
            Assert.IsFalse(filter.Contains("application/binary"));
        }
    }
}

