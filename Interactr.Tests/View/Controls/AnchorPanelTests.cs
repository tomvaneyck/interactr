using Interactr.View.Controls;
using Interactr.View.Framework;
using NUnit.Framework;
namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class AnchorPanelTests
    {
        private AnchorPanel _anchorPanel;
        private UIElement _testUiElement1;

        [SetUp]
        public void Before()
        {
            _anchorPanel = new AnchorPanel();
            _testUiElement1 = new UIElement();
        }

        [Test]
        public void ElementSizeReduce()
        {
            _testUiElement1.PreferredWidth = 4;
            _testUiElement1.PreferredHeight = 4;

            _anchorPanel.Height = 3;
            _anchorPanel.Width = 3;
            _anchorPanel.Children.Add(_testUiElement1);

            int expected = 3;
            int actual = _testUiElement1.Height;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ElementFill()
        {
            _testUiElement1.PreferredWidth = 3;
            _testUiElement1.PreferredHeight = 3;

            _anchorPanel.Height = 4;
            _anchorPanel.Width = 4;
            _anchorPanel.Children.Add(_testUiElement1);

            int expected = 4;
            int actual = _testUiElement1.Height;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ElementNotResize()
        {
            _testUiElement1.PreferredWidth = 3;
            _testUiElement1.PreferredHeight = 3;

            _anchorPanel.Height = 4;
            _anchorPanel.Width = 4;
            _anchorPanel.Children.Add(_testUiElement1);
            AnchorPanel.AnchorsProperty.SetValue(_testUiElement1, Anchors.Top);
            int expected = 3;
            int actual = _testUiElement1.Height;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AutoCompactedIsOff()
        {
            // Turn of autocompact.
            _anchorPanel.AutoCompactEnabled = false;

            // Set the prefered size of the uiElement.
            _testUiElement1.PreferredHeight = 10;
            _testUiElement1.PreferredWidth = 10;

            int expectedPanelPrefHeight = _anchorPanel.PreferredHeight;
            int expectedPanelPrefWidth = _anchorPanel.PreferredWidth;

            _anchorPanel.Children.Add(_testUiElement1);

            // Make sure the preferred size of the anchorPanel did not change.

            int actualPanelPrefHeight = _anchorPanel.PreferredHeight;
            int actualPanelPrefWidth = _anchorPanel.PreferredWidth;

            Assert.AreEqual(expectedPanelPrefWidth, actualPanelPrefWidth);
            Assert.AreEqual(expectedPanelPrefHeight,actualPanelPrefHeight);
        }
    }
}
