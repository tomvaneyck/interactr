using Interactr.View;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using System.Reactive.Concurrency;
using System;
using Interactr.View.Framework;
using Interactr.Reactive;

namespace Interactr.Tests.View
{
    class CommunicationDiagramViewTests
    {
        private TestScheduler _scheduler;
        private TestableCommunicationDiagramView _communicationDiagramView;

        [SetUp]
        public void BeforeEach()
        {
            _scheduler = new TestScheduler();
            _communicationDiagramView = new TestableCommunicationDiagramView();
            _communicationDiagramView.ViewModel =
                new Interactr.ViewModel.CommunicationDiagramViewModel(new Interactr.Model.Diagram());
        }

        [Test]
        public void VisibilityChangeTest()
        {
            int scheduleTicks = 10;
            _scheduler.Schedule(TimeSpan.FromTicks(scheduleTicks),
                () => _communicationDiagramView.ViewModel.IsVisible = true);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, false),
                ReactiveTest.OnNext(10, true)
            };
            var actual = _scheduler.Start(() => _communicationDiagramView.IsVisibleChanged, 0, 0, 1000).Messages;

            // Assert that the visibility changed as expected.
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void AddPartyOnDoubleMouseClick()
        {
            _communicationDiagramView.Focus();
            _communicationDiagramView.RunOnMouseEvent(new MouseEventData(Window.MouseEvent.MOUSE_CLICKED,
                new Point(0, 2), 2));

            // Assert the amount of parties has increased from 0 to 1.
            Assert.AreEqual(_communicationDiagramView.PartyViews.Count, 1);
        }

        private class TestableCommunicationDiagramView : CommunicationDiagramView
        {
            public IReadOnlyReactiveList<CommunicationDiagramPartyView> PartyViews => PartyViewsDragPanel.PartyViews;

            public void RunOnMouseEvent(MouseEventData e)
            {
                OnMouseEvent(e);
            }
        }
    }
}