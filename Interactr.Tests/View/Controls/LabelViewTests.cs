using System;
using System.Diagnostics;
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
        public void EscKeyFunctionalityShouldWork()
        {
            KeyEventData keyEventData = new KeyEventData(KeyEvent.KEY_RELEASED, KeyEvent.VK_ESCAPE, '\x1b');
            _labelView.CanLeaveEditMode = true;

            bool result = _labelView.RunOnKeyEvent(keyEventData);

            // Check if an action occured.
            Assert.IsTrue(result);

            //Check if expected ESC action occurred.
            Assert.IsFalse(_labelView.IsInEditMode);
        }

        public class TestableLabelView : LabelView
        {
            public bool RunOnKeyEvent(KeyEventData keyEventData)
            {
                return OnKeyEvent(keyEventData);
            }
        }
    }

    [TestFixture]
    public class LabelViewObservablesTests
    {
        private LabelViewTests.TestableLabelView _labelView;
        private TestScheduler _scheduler;

        [SetUp]
        public void Before()
        {
            _labelView = new LabelViewTests.TestableLabelView();
            _scheduler = new TestScheduler();
            _labelView.IsInEditMode = true;
        }

        [Test]
        public void LabelTextChangesWhenOneKeyTyped()
        {
            ScheduleTypeTextInLabelView("TEST");
            var expected = new[]
            {
                ReactiveTest.OnNext(1, ""),
                ReactiveTest.OnNext(10,"T"),
                ReactiveTest.OnNext(20,"TE"),
                ReactiveTest.OnNext(30,"TES"),
                ReactiveTest.OnNext(40,"TEST")
            };
            var actual = _scheduler.Start(() => _labelView.TextChanged, 0, 0, 1000).Messages;
            
            ReactiveAssert.AreElementsEqual(expected,actual);
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
                _scheduler.Schedule(TimeSpan.FromTicks(scheduleTicks), () => _labelView.RunOnKeyEvent(keyEventData));
                scheduleTicks += 10;
            }
        }
    }
}