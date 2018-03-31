using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Point = Interactr.View.Framework.Point;

namespace Interactr.View
{
    public class SequenceDiagramMessageView : AnchorPanel
    {
        #region SequenceDiagramMessageViewModel

        private readonly ReactiveProperty<SequenceDiagramMessageViewModel> _viewModel = new ReactiveProperty<SequenceDiagramMessageViewModel>();

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
            Children.Add(_arrow);
            Children.Add(_label);
            AnchorsProperty.SetValue(_label, Anchors.Left | Anchors.Top);

            // Put the arrow starting point on the sender activation bar.
            ObserveActivationBarPosition(vm => vm.SenderActivationBarChanged)
                .Select(activationBarView =>
                {
                    SequenceDiagramView parent = (SequenceDiagramView)Parent;
                    Point anchorPoint = new Point(
                        0,
                        (ViewModel.Tick - activationBarView.ViewModel.StartTick) * activationBarView.TickHeight
                    );
                    Point translatedAnchorPoint = activationBarView.TranslatePointTo(parent, anchorPoint);
                    int xOffset = translatedAnchorPoint.X < _arrow.EndPoint.X ? activationBarView.Width : 0;
                    return translatedAnchorPoint + new Point(xOffset, 0);
                }).Subscribe(newStartPoint => _arrow.StartPoint = newStartPoint);

            // Put the arrow ending point on the receiver activation bar.
            ObserveActivationBarPosition(vm => vm.ReceiverActivationBarChanged)
                .Select(activationBarView =>
                {
                    SequenceDiagramView parent = (SequenceDiagramView)Parent;
                    Point anchorPoint = new Point(
                        0,
                        (ViewModel.Tick - activationBarView.ViewModel.StartTick) * activationBarView.TickHeight
                    );
                    Point translatedAnchorPoint = activationBarView.TranslatePointTo(parent, anchorPoint);
                    int xOffset = translatedAnchorPoint.X < _arrow.StartPoint.X ? activationBarView.Width : 0;
                    return translatedAnchorPoint + new Point(xOffset, 0);
                }).Subscribe(newEndPoint => _arrow.EndPoint = newEndPoint);

            // Put the label central under the arrow.
            ViewModelChanged.ObserveNested(vm => vm.LabelChanged).Subscribe(label => _label.Text = label);
            Observable.CombineLatest(
                _arrow.StartPointChanged,
                _arrow.EndPointChanged
            ).Subscribe(p =>
            {
                Point start = p[0].X < p[1].X ? p[0] : p[1];
                Point end = p[0].X > p[1].X ? p[0] : p[1];
                Point diff = end - start;
                Point textPos = start + new Point(diff.X / 3, diff.Y / 3);
                MarginsProperty.SetValue(_label, new Margins(textPos.X, textPos.Y));
            });
        }

        private IObservable<ActivationBarView> ObserveActivationBarPosition(Func<SequenceDiagramMessageViewModel, IObservable<ActivationBarViewModel>> barSelector)
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
