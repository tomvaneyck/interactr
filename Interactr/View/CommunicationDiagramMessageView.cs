using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;

namespace Interactr.View
{
    public class CommunicationDiagramMessageView : AnchorPanel
    {
        #region CommunicationDiagramMessageViewModel

        private readonly ReactiveProperty<CommunicationDiagramMessageViewModel> _viewModel =
            new ReactiveProperty<CommunicationDiagramMessageViewModel>();

        public CommunicationDiagramMessageViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<CommunicationDiagramMessageViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        private readonly ArrowView _arrow = new ArrowView();
        private readonly LabelView _label = new LabelView();

        public CommunicationDiagramMessageView()
        {
            Children.Add(_arrow);
            Children.Add(_label);
            AnchorsProperty.SetValue(_label, Anchors.Left | Anchors.Top);

            // Put the arrow starting point on the sender.
            ObservePartyPosition(vm => vm.Message.SenderChanged)
                .Select(partyview => partyview.Position)
                .Subscribe(newStartPoint => _arrow.StartPoint = newStartPoint);

            // Put the arrow ending point on the receiver.
            ObservePartyPosition(vm => vm.Message.ReceiverChanged)
                .Select(partyView => partyView.Position)
                .Subscribe(newEndPoint => _arrow.EndPoint = newEndPoint);

            // Change the size of the arrow views.
            WidthChanged.Subscribe(newWidth =>
                _arrow.PreferredWidth = newWidth);

            HeightChanged.Subscribe(newHeight => _arrow.PreferredHeight = newHeight);

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

        private IObservable<PartyView> ObservePartyPosition(
            Func<CommunicationDiagramMessageViewModel, IObservable<Party>> partySelector)
        {
            // Select the latest parent view
            return ParentChanged.OfType<CommunicationDiagramView>().Select(parent =>
                // and the latest viewmodel
                    ViewModelChanged.Where(vm => vm != null).Select(vm =>
                        // and the latest matching sender
                            partySelector(vm).Where(party => party != null).Select(targetParty =>
                            {
                                // and listen for the position changes of its view.
                                return parent.PartyViews.ObserveWhere(
                                    party => party.PositionChanged,
                                    party => party.ViewModel.Party == targetParty).Select(e => e.Element);
                            }).Switch()
                    ).Switch()
            ).Switch();
        }
    }
}