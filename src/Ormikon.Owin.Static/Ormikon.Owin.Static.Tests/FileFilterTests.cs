using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ormikon.Owin.Static.Tests
{
    [TestClass]
    public class FileFilterTests
    {
        [TestMethod]
        public void ExactMatchFile()
        {
            var ff = new FileFilter("concrete-name.txt");
            Assert.IsTrue(ff.Contains("d:/concrete-name.txt"));
            Assert.IsTrue(ff.Contains("d:/anotherfolder/concrete-name.txt"));
            Assert.IsFalse(ff.Contains("d:/notaconcrete-name.txt"));
            Assert.IsFalse(ff.Contains("d:/concrete-name1.txt"));
            Assert.IsFalse(ff.Contains("d:/concrete-name.txty"));
            Assert.IsFalse(ff.Contains("d:/concrete-namme.txt"));
        }

        [TestMethod]
        public void ExactMatchFileAndDir()
        {
            var ff = new FileFilter("concrete-dir/concrete-name.txt");
            Assert.IsTrue(ff.Contains("d:/concrete-dir/concrete-name.txt"));
            Assert.IsTrue(ff.Contains("d:/anotherdir/concrete-dir/concrete-name.txt"));
            Assert.IsFalse(ff.Contains("d:/concrete-dir/concrete-nam1e.txt"));
            Assert.IsFalse(ff.Contains("d:/concrete-dimr/concrete-name.txt"));
            Assert.IsFalse(ff.Contains("d:/concrete-dir/concrete-name.txtd"));
            Assert.IsFalse(ff.Contains("d:/concrete-dird/concrete-name.txt"));
            Assert.IsFalse(ff.Contains("d:/concrete-dir/sconcrete-name.txt"));
            Assert.IsFalse(ff.Contains("d:/sconcrete-dir/concrete-name.txt"));
            Assert.IsFalse(ff.Contains("d:/concrete-dir/anotherdir/concrete-name.txt"));
        }

        [TestMethod]
        public void TestQuestionMark()
        {
            var ff = new FileFilter("file-with-qm.t?t");
            Assert.IsTrue(ff.Contains("file-with-qm.txt"));
            Assert.IsTrue(ff.Contains("file-with-qm.tmt"));
            Assert.IsFalse(ff.Contains("file-with-qm.tt"));
            Assert.IsFalse(ff.Contains("file-with-qm.ttx"));

            ff = new FileFilter("file-with-qm.tx?");
            Assert.IsTrue(ff.Contains("file-with-qm.txt"));
            Assert.IsTrue(ff.Contains("file-with-qm.txm"));
            Assert.IsFalse(ff.Contains("file-with-qm.tx"));
            Assert.IsFalse(ff.Contains("file-with-qm.tm"));
        }

        [TestMethod]
        public void TestOneStar()
        {
            var ff = new FileFilter("*/*test.xml");
            Assert.IsTrue(ff.Contains("d:/tempdir/anotherdir/yytest.xml"));
            Assert.IsTrue(ff.Contains("d:/tempdir/anotherdir/test.xml"));
            Assert.IsTrue(ff.Contains("d:/mmtest.xml"));
            Assert.IsFalse(ff.Contains("d:/tempdir/anotherdir/fest.xml"));

            ff = new FileFilter("te*st.txt");
            Assert.IsTrue(ff.Contains("test.txt"));
            Assert.IsTrue(ff.Contains("tehst.txt"));
            Assert.IsTrue(ff.Contains("testkljlkjlst.txt"));
            Assert.IsTrue(ff.Contains("test.txt.kljlkjlst.txt"));
            Assert.IsFalse(ff.Contains("temdsgt.txt"));

            ff = new FileFilter("dir1/*/file.txt");
            Assert.IsTrue(ff.Contains("dir1/dir2/file.txt"));
            Assert.IsFalse(ff.Contains("dir1/file.txt"));
            Assert.IsFalse(ff.Contains("dir1file.txt"));
        }

        [TestMethod]
        public void TestDoubleStar()
        {
            var ff = new FileFilter("/one/**/another.txt");
            Assert.IsTrue(ff.Contains("one/another.txt"));
            Assert.IsTrue(ff.Contains("one/subdir/another.txt"));
            Assert.IsTrue(ff.Contains("one/subdir/anothersubdir/another.txt"));
            Assert.IsTrue(ff.Contains("one/subdir/anothersubdir/onemoresubdir/another.txt"));
            Assert.IsFalse(ff.Contains("another/another.txt"));
            Assert.IsFalse(ff.Contains("one1/another.txt"));
            Assert.IsFalse(ff.Contains("one/1another.txt"));
            Assert.IsFalse(ff.Contains("oneanother.txt"));

            ff = new FileFilter("/one/**/two/**/three.mht");
            Assert.IsTrue(ff.Contains("/one/two/three.mht"));
            Assert.IsTrue(ff.Contains("/one/two/sss/three.mht"));
            Assert.IsTrue(ff.Contains("/one/ssss/two/three.mht"));
            Assert.IsTrue(ff.Contains("/one/vvv/two/mmm/three.mht"));
            Assert.IsTrue(ff.Contains("/one//fff/ggg/two/sss/bbbb/three.mht"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidChars()
        {
// ReSharper disable ObjectCreationAsStatement
            new FileFilter("inva>lid");
// ReSharper restore ObjectCreationAsStatement
        }
    }
}
