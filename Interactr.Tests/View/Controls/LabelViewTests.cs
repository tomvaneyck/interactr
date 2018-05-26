using System;
using System.Reactive.Concurrency;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.Window;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class LabelViewInputTests
    {
        private LabelView _labelView;
        private TestScheduler _scheduler;

        [SetUp]
        public void BeforeEach()
        {
            _labelView = new LabelView();
            _labelView.Focus();
            _scheduler = new TestScheduler();
        }

        [Test]
        [Category("RequiresUI")]
        public void EscKeyFunctionalityEvent()
        {
            KeyEventData keyEventData = new KeyEventData(KeyEvent.KEY_RELEASED,
                KeyEvent.VK_ESCAPE, '\x1b');
            _labelView.CanLeaveEditMode = true;
            _labelView.IsInEditMode = true;

            UIElement.HandleKeyEvent(keyEventData);

            // Check if an action was handled.
            Assert.IsTrue(keyEventData.IsHandled);

            // Check if expected ESC action occurred.
            Assert.IsFalse(_labelView.IsInEditMode);
        }

        [Test]
        [Category("RequiresUI")]
        public void EscKeyFunctionalityNoEvent()
        {
            KeyEventData keyEventData = new KeyEventData(KeyEvent.KEY_RELEASED, KeyEvent.VK_ESCAPE, '\x1b');
            _labelView.CanLeaveEditMode = true;
            _labelView.IsInEditMode = true;
            UIElement.HandleKeyEvent(keyEventData);

            // Check if an action was handled.
            Assert.IsTrue(keyEventData.IsHandled);

            // Check if expected ESC action occurred.
            Assert.IsFalse(_labelView.IsInEditMode);
        }

        [Test]
        public void EscKeyFunctionalityNotInEditMode()
        {
            KeyEventData keyEventData = new KeyEventData(KeyEvent.KEY_RELEASED,
                KeyEvent.VK_ESCAPE, '\x1b');
            _labelView.CanLeaveEditMode = true;
            UIElement.HandleKeyEvent(keyEventData);

            // Check if expected ESC action occurred.
            Assert.IsFalse(_labelView.IsInEditMode);
        }


        [Test]
        public void MouseClickFunctionalityEventOutsideLabel()
        {
            _labelView.Position = new Point(0, 0);
            _labelView.CanLeaveEditMode = true;
            MouseEventData mouseEventData =
                new MouseEventData(MouseEvent.MOUSE_CLICKED, new Point(_labelView.Width + 1, _labelView.Height + 1), 1);

            UIElement.HandleMouseEvent(_labelView, mouseEventData);

            // Edit mode should still be false.
            Assert.IsFalse(_labelView.IsInEditMode);
        }

        [Test]
        public void MouseClickFunctionalityEventNoClick()
        {
            _labelView.Position = new Point(0, 0);
            _labelView.CanLeaveEditMode = true;

            Point pointInLabel = new Point(_labelView.Width / 2, _labelView.Height / 2);
            MouseEventData mouseEventData =
                new MouseEventData(MouseEvent.NOBUTTON, pointInLabel, 1);

            UIElement.HandleMouseEvent(_labelView, mouseEventData);

            // Check if expected mouse action occured.
            Assert.IsFalse(_labelView.IsInEditMode);
        }

        [Test]
        [Category("RequiresUI")]
        public void LabelTextChangesWhenOneKeyTyped()
        {
            _labelView.IsInEditMode = true;
            ScheduleTypeTextInLabelView("TEST");
            var expected = new[]
            {
                ReactiveTest.OnNext(1, ""),
                ReactiveTest.OnNext(10, "T"),
                ReactiveTest.OnNext(20, "TE"),
                ReactiveTest.OnNext(30, "TES"),
                ReactiveTest.OnNext(40, "TEST")
            };
            var actual = _scheduler.Start(() => _labelView.TextChanged, 0, 0, 1000).Messages;

            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        [Category("RequiresUI")]
        public void TestBackSpaceInLabelView()
        {
            _labelView.IsInEditMode = true;
            ScheduleTypeTextInLabelView("TEST\bA");
            var expected = new[]
            {
                ReactiveTest.OnNext(1, ""),
                ReactiveTest.OnNext(10, "T"),
                ReactiveTest.OnNext(20, "TE"),
                ReactiveTest.OnNext(30, "TES"),
                ReactiveTest.OnNext(40, "TEST"),
                ReactiveTest.OnNext(50, "TES"),
                ReactiveTest.OnNext(60, "TESA")
            };
            var actual = _scheduler.Start(() => _labelView.TextChanged, 0, 0, 1000).Messages;

            // Assertion
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void BackSpaceWithNoTextInLabelViewShouldNotChangeText()
        {
            ScheduleTypeTextInLabelView("\b");
            var expected = new[]
            {
                ReactiveTest.OnNext(1, "")
            };
            var actual = _scheduler.Start(() => _labelView.TextChanged, 0, 0, 1000).Messages;

            // Assertion
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        /// <summary>
        /// Helper function for faking the typing of characters.
        /// A character is typed every 10 ticks.
        /// </summary>
        /// <param name="text">The text to schedule type events for.</param>
        public void ScheduleTypeTextInLabelView(string text)
        {
            int scheduleTicks = 10;
            // Schedule to type a character in every 10 ticks.
            foreach (char character in text)
            {
                KeyEventData keyEventData = new KeyEventData(KeyEvent.KEY_TYPED, KeyEvent.CHAR_UNDEFINED, character);

                _scheduler.Schedule(TimeSpan.FromTicks(scheduleTicks), () => UIElement.HandleKeyEvent(keyEventData));

                scheduleTicks += 10;
            }
        }
    }
}