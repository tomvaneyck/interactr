using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            Assert.Pass("My first test");
        }

        [Test]
        public void TestFail()
        {
            Assert.Fail();
        }
    }
}
