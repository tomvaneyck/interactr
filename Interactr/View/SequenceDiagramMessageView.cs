using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Point = Interactr.View.Framework.Point;
using LineType = Interactr.View.Controls.LineView.LineType;
using Message = Interactr.Model.Message;

namespace Interactr.View
{
    public class SequenceDiagramMessageView : AnchorPanel
    {
        private static readonly Color DefaultLabelColor = Color.Black;
        private static readonly Color InvalidLabelColor = Color.Red;

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

        /// <summary>
        /// The messageNumber view of the message.
        /// </summary>
        public LabelWithMessageNumberView LabelWithMessageNumberView { get; } = new LabelWithMessageNumberView();

        private readonly ArrowView _arrow = new ArrowView();

        public SequenceDiagramMessageView()
        {
            this.IsVisibleToMouse = false;

            Children.Add(_arrow);
            Children.Add(LabelWithMessageNumberView);

            AnchorsProperty.SetValue(LabelWithMessageNumberView, Anchors.Left | Anchors.Top);

            // Bidirectionally bind the view label to the label in the viewmodel.
            ViewModelChanged.ObserveNested(vm => vm.LabelChanged)
                .Subscribe(label => LabelWithMessageNumberView.LabelView.Text = label);

            LabelWithMessageNumberView.LabelView.TextChanged.Subscribe(text =>
            {
                if (ViewModel != null)
                {
                    ViewModel.Label = text;
                }

                // Center the text on a text change.
                var textSize = TextRenderer.MeasureText(LabelWithMessageNumberView.WholeText,
                    LabelWithMessageNumberView.LabelView.Font);
                Point textPos = _arrow.calculateMidPoint() - new Point(textSize.Width / 2, 0);

                // Set the labelMessageNumber view margins.
                MarginsProperty.SetValue(LabelWithMessageNumberView,
                    new Margins(textPos.X, textPos.Y));
            });

            // Put the arrow starting point on the sender activation bar.
            ObserveActivationBarPosition(vm => vm.SenderActivationBarChanged)
                .Select(activationBarView => GetArrowAnchorPoint(activationBarView, _arrow.EndPoint))
                .Subscribe(newStartPoint => _arrow.StartPoint = newStartPoint);

            // Put the arrow ending point on the receiver activation bar.
            ObserveActivationBarPosition(vm => vm.ReceiverActivationBarChanged)
                .Select(activationBarView => GetArrowAnchorPoint(activationBarView, _arrow.StartPoint))
                .Subscribe(newEndPoint => _arrow.EndPoint = newEndPoint);

            // Set the display style of the arrow.
            ViewModelChanged.Where(vm => vm != null).Subscribe(vm =>
            {
                _arrow.Style = vm.MessageType == Message.MessageType.Invocation ? LineType.Solid : LineType.Dotted;
            });

            // Bind the message number of the view to the viewmodel and adjust
            // the height and width of the messageNumberView.
            ViewModelChanged.ObserveNested(vm => vm.MessageNumberChanged)
                .Subscribe(m => LabelWithMessageNumberView.SetMessageNumber(m));

            // The label is red if CanApplyLabel is true.
            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged).Subscribe(canApplyLabel =>
                LabelWithMessageNumberView.LabelView.Color = canApplyLabel
                                                             || ViewModel.MessageType == Message.MessageType.Result
                    ? DefaultLabelColor
                    : InvalidLabelColor);

            // Fire ApplyLabel when leaving edit mode.
            LabelWithMessageNumberView.LabelView.EditModeChanged.Subscribe(
                isInEditMode =>
                {
                    if (ViewModel != null && ViewModel.MessageType == Message.MessageType.Result)
                    {
                        ViewModel.ApplyLabel();
                    }
                    else if (ViewModel != null && !isInEditMode)
                    {
                        ViewModel.ApplyLabel();
                    }
                }
            );

            Observable.CombineLatest(
                _arrow.StartPointChanged,
                _arrow.EndPointChanged
            ).Subscribe(p =>
            {
                // Center the label.
                var textSize = TextRenderer.MeasureText(LabelWithMessageNumberView.WholeText,
                    LabelWithMessageNumberView.LabelView.Font);
                Point textPos = _arrow.calculateMidPoint() - new Point(textSize.Width / 2, 0);

                MarginsProperty.SetValue(LabelWithMessageNumberView,
                    new Margins(textPos.X, textPos.Y));
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
            SequenceDiagramView parent = (SequenceDiagramView) Parent;
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