using Interactr.View.Controls;
using Interactr.View.Framework;
using NUnit.Framework;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class WindowsViewTests
    {
        [Test]
        public void TestAddWindow1()
        {
            RectangleView r = new RectangleView();
            WindowsView wv = new WindowsView();
            WindowsView.Window wvNew = wv.AddWindow(r);
            Assert.AreEqual(500,wvNew.Height);
        }
        
        [Test]
        public void TestAddWindow2()
        {
            RectangleView r = new RectangleView();
            WindowsView wv = new WindowsView();
            WindowsView.Window wvNew = wv.AddWindow(r);
            Assert.AreEqual(500,wvNew.Width);
        }
        
        [Test]
        public void TestAddWindow3()
        {
            RectangleView r1 = new RectangleView();
            RectangleView r2 = new RectangleView();
            WindowsView wv = new WindowsView();
            WindowsView.Window wv1 = wv.AddWindow(r1); // To get an offset on wv2
            WindowsView.Window wv2 = wv.AddWindow(r2);
            Assert.AreEqual(new Point(10, 10), wv2.Position);
        }
        
        [Test]
        public void TestRemoveWindow()
        {
            RectangleView r1 = new RectangleView();
            RectangleView r2 = new RectangleView();
            WindowsView wv = new WindowsView();
            WindowsView.Window wv1 = wv.AddWindow(r1); // To get an offset on wv2
            WindowsView.Window wv2 = wv.AddWindow(r2);
            wv.RemoveWindowWith(r2);
            Assert.AreEqual(false,wv.Children.Contains(wv2));
        } 
    }
}