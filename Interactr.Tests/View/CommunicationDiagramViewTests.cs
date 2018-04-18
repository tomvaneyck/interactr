using Interactr.View;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using System.Reactive.Concurrency;
using System;

namespace Interactr.Tests.View
{
    class CommunicationDiagramViewTests
    {

        private TestScheduler _scheduler;
        private CommunicationDiagramView _communicationDiagramView;

        [SetUp]
        public void BeforeEach()
        {
            _scheduler = new TestScheduler();
            _communicationDiagramView = new CommunicationDiagramView();
            _communicationDiagramView.ViewModel = new Interactr.ViewModel.CommunicationDiagramViewModel(new Interactr.Model.Diagram());
        }

        [Test]
        public void TestVisibilityChange()
        {
            int scheduleTicks = 10;
            _scheduler.Schedule(TimeSpan.FromTicks(scheduleTicks), () => _communicationDiagramView.ViewModel.IsVisible = true);
            
            var expected = new[]
            {
                ReactiveTest.OnNext(1, false),
                ReactiveTest.OnNext(10, true)
            };
            var actual = _scheduler.Start(() => _communicationDiagramView.IsVisibleChanged, 0, 0, 1000).Messages;

            ReactiveAssert.AreElementsEqual(expected, actual);
        }


    }
}
