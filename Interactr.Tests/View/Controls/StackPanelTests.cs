using NUnit.Framework;
using Interactr.View.Controls;
using Interactr.View.Framework;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class StackPanelTests
    {
        [Test]
        public void TestHorizontalStacking()
        {
            UIElement elem1 = new UIElement
            {
                PreferredWidth = 50,
                PreferredHeight = 50
            };
            UIElement elem2 = new UIElement
            {
                PreferredWidth = 100,
                PreferredHeight = 100
            };

            StackPanel stackPanel = new StackPanel
            {
                StackOrientation = Orientation.Horizontal,
                Width = 500,
                Height = 500,
                Children = { elem1, elem2 }
            };

            Assert.AreEqual(50, elem1.Width);
            Assert.AreEqual(500, elem1.Height);
            Assert.AreEqual(100, elem2.Width);
            Assert.AreEqual(500, elem2.Height);
        }

        [Test]
        public void TestVerticalStacking()
        {
            UIElement elem1 = new UIElement
            {
                PreferredWidth = 50,
                PreferredHeight = 50
            };
            UIElement elem2 = new UIElement
            {
                PreferredWidth = 100,
                PreferredHeight = 100
            };
            
            StackPanel stackPanel = new StackPanel
            {
                StackOrientation = Orientation.Vertical,
                Width = 500,
                Height = 500,
                Children = { elem1, elem2 }
            };

            Assert.AreEqual(500, elem1.Width);
            Assert.AreEqual(50, elem1.Height);
            Assert.AreEqual(500, elem2.Width);
            Assert.AreEqual(100, elem2.Height);
        }
    }
}
