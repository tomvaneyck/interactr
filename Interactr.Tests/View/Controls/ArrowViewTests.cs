using System;
using NUnit.Framework;
using Interactr.View.Controls;
using Interactr.View.Framework;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class ArrowViewTests
    {
        [Test]
        public void CalculateWingPointsTest1()
        {
            ArrowView av = new ArrowView();
            av.ArrowHeadSize = 10;
            av.StartPoint = new Point(0, 0);
            av.EndPoint = new Point(1, 1);
            Assert.AreEqual((new Point(1,-9),new Point(-9,1)),av.CalculateWingPoints());
        }

        [Test]
        public void CalculateWingPointsTest2()
        {
            ArrowView av = new ArrowView();
            av.ArrowHeadSize = 10;
            av.StartPoint = new Point(0, 0);
            av.EndPoint = new Point(3, 2);
            Assert.AreEqual((new Point(1, -8), new Point(-7, 4)), av.CalculateWingPoints());
        }

        [Test]
        public void CalculateWingPointsTest3()
        {
            ArrowView av = new ArrowView();
            av.ArrowHeadSize = 10;
            av.StartPoint = new Point(0, 0);
            av.EndPoint = new Point(-1, -1);
            Assert.AreEqual((new Point(-1, 9), new Point(9, -1)), av.CalculateWingPoints());
        }

        [Test]
        public void ArroViewTest()
        {
            ArrowView av = new ArrowView();
            Assert.AreEqual(10,av.ArrowHeadSize);
        }

        [Test]
        public void StartPointTest()
        {
            ArrowView av = new ArrowView();
            av.StartPoint = new Point(1, 1);
            Assert.AreEqual(new Point(1, 1), av.StartPoint);
        }
    }
}
