using System.Drawing;
using Interactr.View.Controls;
using NUnit.Framework;
using Point = Interactr.View.Framework.Point;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class LineViewTests
    {
        [Test]
        public void TestPointB()
        {
            LineView lv = new LineView();
            Assert.AreEqual(new Point(10,10),lv.PointB);
        }
        
        [Test]
        public void TestObservableMergeWidth1()
        {
            LineView lv = new LineView();
            Assert.AreEqual(Pens.Black, lv.Pen);
            lv.PointB = new Point(9, 9);
            Assert.AreEqual(9,lv.PreferredWidth);
        }
        
        [Test]
        public void TestObservableMergeHeight1()
        {
            LineView lv = new LineView();
            Assert.AreEqual(Pens.Black, lv.Pen);
            lv.PointB = new Point(9, 9);
            Assert.AreEqual(9,lv.PreferredHeight);
        }
        
        [Test]
        public void TestObservableMergeWidth2()
        {
            LineView lv = new LineView();
            Assert.AreEqual(Pens.Black, lv.Pen);
            lv.PointB = new Point(11, 11);
            Assert.AreEqual(11,lv.PreferredWidth);
        }
        
        [Test]
        public void TestObservableMergeHeight2()
        {
            LineView lv = new LineView();
            Assert.AreEqual(Pens.Black, lv.Pen);
            lv.PointB = new Point(11, 11);
            Assert.AreEqual(11,lv.PreferredHeight);
        }
        
    }
}