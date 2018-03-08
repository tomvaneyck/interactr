using System.Reactive.Concurrency;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.Window;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class LabelViewTests
    {
        private TestableLabelView _labelView;
        private TestScheduler _scheduler;

        [SetUp]
        public void BeforeEach()
        {
            _labelView = new TestableLabelView();
            _scheduler = new TestScheduler();
        }

        [Test]
        public void EscKeyFunctionalityEvent()
        {
            KeyEventData keyEventData = new KeyEventData(KeyEvent.KEY_RELEASED, KeyEvent.VK_ESCAPE, '\x1b');
            _labelView.CanLeaveEditMode = true;
            // set to true because the default is false
            _labelView.IsInEditMode = true;

            bool result = _labelView.RunOnKeyEvent(keyEventData);

            // Check if an action occurred.
            Assert.IsTrue(result);

            // Check if expected ESC action occurred.
            Assert.IsFalse(_labelView.IsInEditMode);
        }

        public class TestableLabelView : LabelView
        {
            public bool RunOnKeyEvent(KeyEventData keyEventData)
            {
                return OnKeyEvent(keyEventData);
            }
        }

        [Test]
        public void EscKeyFunctionalityNoEvent()
        {
            KeyEventData keyEventData = new KeyEventData(KeyEvent.KEY_RELEASED, -1, '\x1b');

            _labelView.CanLeaveEditMode = true;
            _labelView.IsInEditMode = true;

            bool result = _labelView.RunOnKeyEvent(keyEventData);

            // Check if an action occurred.
            Assert.IsFalse(result);

            // Check if expected ESC action occurred.
            Assert.IsTrue(_labelView.IsInEditMode);
        }

        [Test]
        public void EscKeyFunctionalityDontEnterEditMode()
        {
            KeyEventData keyEventData = new KeyEventData(KeyEvent.KEY_RELEASED, KeyEvent.VK_ESCAPE, '\x1b');
            _labelView.CanLeaveEditMode = true;

            bool result = _labelView.RunOnKeyEvent(keyEventData);

            // Check if an action occurred.
            Assert.IsTrue(result);

            // Check if expected ESC action occurred.
            Assert.IsFalse(_labelView.IsInEditMode);
        }

        [Test]
        public void MouseClickFunctionalityEventInLabel()
        {
            LabelView labelview = new LabelView();
            MouseEventData mouseEventData = new MouseEventData(MouseEvent.MOUSE_CLICKED, new Point(0, 0), 1);

            bool result = UIElement.HandleMouseEvent(labelview, mouseEventData);

            // Check if an action occurred.
            Assert.IsTrue(result);

            // Check if expected mouse action occured.
            Assert.IsTrue(_labelView.IsInEditMode);
        }

        [Test]
        public void MouseClickFunctionalityEventOutsideLabel()
        {
            UIElement ui = new UIElement();
            LabelView labelview = new LabelView();
            labelview.Height = 2;
            labelview.Width = 2;
            ui.Children.Add(labelview);
            MouseEventData mouseEventData = new MouseEventData(MouseEvent.MOUSE_CLICKED, new Point(int.MaxValue, int.MaxValue), 1);

            bool result = UIElement.HandleMouseEvent(ui, mouseEventData);

            // Check if an action occurred.
            Assert.IsFalse(result);

            // Check if expected mouse action occured.
            Assert.IsFalse(_labelView.IsInEditMode);
        }

        [Test]
        public void MouseClickFunctionalityEventNoClick()
        {
            LabelView labelview = new LabelView();
            MouseEventData mouseEventData = new MouseEventData(MouseEvent.NOBUTTON, new Point(int.MaxValue, int.MaxValue), 1);

            bool result = UIElement.HandleMouseEvent(labelview, mouseEventData);

            // Check if an action occurred.
            Assert.IsFalse(result);

            // Check if expected mouse action occured.
            Assert.IsFalse(_labelView.IsInEditMode);
        }
    }
}