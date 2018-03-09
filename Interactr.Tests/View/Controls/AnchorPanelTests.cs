using System;
using Interactr.View.Controls;
using Interactr.View.Framework;
using NUnit.Framework;
namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class AnchorPanelTests
    {
        
        [Test]
        public void ElementSizeReduce()
        {
            UIElement el = new UIElement();
            el.PreferredWidth = 4;
            el.PreferredHeight = 4;

            AnchorPanel ap = new AnchorPanel();
            ap.Height = 3;
            ap.Width = 3;
            ap.Children.Add(el);

            int expected = 3;
            int actual = el.Height;
            Assert.AreEqual(expected,actual);

        }

        [Test]
        public void ElementFill()
        {
            UIElement el = new UIElement();
            el.PreferredWidth = 3;
            el.PreferredHeight = 3;

            AnchorPanel ap = new AnchorPanel();
            ap.Height = 4;
            ap.Width = 4;
            ap.Children.Add(el);

            int expected = 4;
            int actual = el.Height;
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void ElementNotResize()
        {
            UIElement el = new UIElement();
            el.PreferredWidth = 3;
            el.PreferredHeight = 3;

            AnchorPanel ap = new AnchorPanel();
            ap.Height = 4;
            ap.Width = 4;
            ap.Children.Add(el);
            AnchorPanel.AnchorsProperty.SetValue(el,Anchors.Top);
            int expected = 3;
            int actual = el.Height;
            Assert.AreEqual(expected, actual);
        }
    }
}
