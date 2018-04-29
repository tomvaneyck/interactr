using System;
using System.Linq;
using System.Reactive.Linq;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.ViewModel;
using Point = Interactr.View.Framework.Point;

namespace Interactr.View
{
    public class SequenceDiagramMessageView : AnchorPanel
    {
        #region SequenceDiagramMessageViewModel

        private readonly ReactiveProperty<SequenceDiagramMessageViewModel> _viewModel =
            new ReactiveProperty<SequenceDiagramMessageViewModel>();

        public SequenceDiagramMessageViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<SequenceDiagramMessageViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        private readonly ArrowView _arrow = new ArrowView();
        private readonly LabelView _label = new LabelView();

        public SequenceDiagramMessageView()
        {
            this.IsVisibleToMouse = false;

            Children.Add(_arrow);
            Children.Add(_label);
            AnchorsProperty.SetValue(_label, Anchors.Left | Anchors.Top);

            // Put the arrow starting point on the sender activation bar.
            ObserveActivationBarPosition(vm => vm.SenderActivationBarChanged)
                .Select(activationBarView => GetArrowAnchorPoint(activationBarView, _arrow.EndPoint))
                .Subscribe(newStartPoint => _arrow.StartPoint = newStartPoint);

            // Put the arrow ending point on the receiver activation bar.
            ObserveActivationBarPosition(vm => vm.ReceiverActivationBarChanged)
                .Select(activationBarView => GetArrowAnchorPoint(activationBarView, _arrow.StartPoint))
                .Subscribe(newEndPoint => _arrow.EndPoint = newEndPoint);

            // Put the label under the arrow.
            ViewModelChanged.ObserveNested(vm => vm.LabelChanged).Subscribe(label => _label.Text = label);
            Observable.CombineLatest(
                _arrow.StartPointChanged,
                _arrow.EndPointChanged
            ).Subscribe(p =>
            {
                // Get the leftmost point
                Point start = p[0].X < p[1].X ? p[0] : p[1];
                // Get the rightmost point
                Point end = p[0].X > p[1].X ? p[0] : p[1];
                // Get the vector from the leftmost to the rightmost point.
                Point diff = end - start;
                // Start the text at a third of the distance between the points. Looks good enough for now.
                Point textPos = start + new Point(diff.X / 3, diff.Y / 3);
                MarginsProperty.SetValue(_label, new Margins(textPos.X, textPos.Y));
            });
        }

        /// <summary>
        /// Calculate the point on the diagram where the arrow should start/stop.
        /// </summary>
        /// <param name="bar">The activation bar that the arrow should attach to.</param>
        /// <param name="otherSideOfArrow">The point that defines the other side of the arrow.</param>
        /// <returns>A point on the diagram view.</returns>
        private Point GetArrowAnchorPoint(ActivationBarView bar, Point otherSideOfArrow)
        {
            Point anchorPointOnBar = new Point(
                0,
                (ViewModel.Tick - bar.ViewModel.StartTick) * bar.TickHeight
            );
            SequenceDiagramView parent = (SequenceDiagramView)Parent;
            Point anchorPointOnDiagram = bar.TranslatePointTo(parent, anchorPointOnBar);

            // Choose left or right side of bar based on which side the arrow is going.
            int xOffset = anchorPointOnDiagram.X > otherSideOfArrow.X ? 0 : bar.Width;
            return anchorPointOnDiagram + new Point(xOffset, 0);
        }

        private IObservable<ActivationBarView> ObserveActivationBarPosition(
            Func<SequenceDiagramMessageViewModel, IObservable<ActivationBarViewModel>> barSelector)
        {
            // With the latest parent view
            return ParentChanged.OfType<SequenceDiagramView>().Select(parent =>
                    // and the latest viewmodel
                    ViewModelChanged.Where(vm => vm != null).Select(vm =>
                            // and the latest matching activation bar
                            barSelector(vm).Where(bar => bar != null).Select(targetBar =>
                            {
                                // and listen for the position changes of its view.
                                return parent.ColumnViews.ObserveWhere(
                                    column => column.LifeLineView.ActivationBarViews.ObserveWhere(
                                        bar => bar.AbsolutePositionChanged,
                                        bar => bar.ViewModel == targetBar),
                                    column => column.PartyView.ViewModel.Party == targetBar.Party
                                ).Select(e => e.Value.Element);
                            }).Switch()
                    ).Switch()
            ).Switch();
        }
    }
}