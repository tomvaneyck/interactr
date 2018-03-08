using System.Reactive.Concurrency;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.Window;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace Interactr.Tests.View.Controls
{
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
}