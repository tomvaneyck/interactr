using System.Reactive.Concurrency;
using Interactr.View.Controls;
using Interactr.View.Framework;
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
        public void TestEscKeyFunctionality()
        {
        }

        public class TestableLabelView : LabelView
        {
            public void RunOnKeyEvent(KeyEventData keyEventData)
            {
                OnKeyEvent(keyEventData);
            }
        }
    }
}