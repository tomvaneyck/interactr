using System.Drawing.Printing;
using NUnit.Framework;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class MarginTests
    {
        [Test]
        public void testEquals()
        {
            Margins m1 = new Margins(1,1,1,1);
            Margins m2 = new Margins(1,1,1,1);
            Assert.AreEqual(true,m1 == m2);
        }
        
        [Test]
        public void testNotEquals()
        {
            Margins m1 = new Margins(1,1,1,1);
            Margins m2 = new Margins(1,2,1,1);
            Assert.AreEqual(true,m1 != m2);
        }
    }
}