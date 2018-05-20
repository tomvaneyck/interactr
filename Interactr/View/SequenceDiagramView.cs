using System;
using System.Linq;
using System.Reactive.Linq;
using Interactr.Constants;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    /// <summary>
    /// The view for the sequence diagram.
    /// </summary>
    public class SequenceDiagramView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<SequenceDiagramViewModel> _viewModel =
            new ReactiveProperty<SequenceDiagramViewModel>();

        public SequenceDiagramViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<SequenceDiagramViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public IReadOnlyReactiveList<SequenceDiagramColumnView> ColumnViews { get; private set; }
        private IReadOnlyReactiveList<SequenceDiagramMessageView> _messageViews;

        /// View for the message currently being dragged by the user.
        private readonly ArrowView _pendingMessageView = new ArrowView
        {
            IsVisibleToMouse = false
        };

        public SequenceDiagramView()
        {
            // The visibility of this view is to be set to the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged)
                .Subscribe(isVisible => { this.IsVisible = isVisible; });

            SetupPartyColumns();
            SetupMessages();
            SetupPendingMessage();

            ColumnViews.ObserveEach(cv => cv.PartyView.LabelView.KeyEventOccurred).Subscribe(e =>
                {
                    var eventData = e.Value;
                    if (eventData.Id == KeyEvent.KEY_PRESSED && eventData.KeyCode == KeyCodes.Delete)
                    {
                        // Delete the party from the viewmodel. This automatically
                        // propagates to the view and the model.
                        ViewModel.DeleteParty(e.Element.PartyView.ViewModel.Party);
                    }
                }
            );

            _messageViews.ObserveEach(mv => mv.LabelWithMessageNumberView.LabelView.KeyEventOccurred).Subscribe(e =>
                {
                    var eventData = e.Value;
                    if (eventData.Id == KeyEvent.KEY_PRESSED && eventData.KeyCode == KeyCodes.Delete)
                    {
                        // Delete the party from the viewmodel. This automatically
                        // propagates to the view and the model.
                        ViewModel.DeleteMessage(e.Element.ViewModel.Message);
                    }
                }
            );
        }

        private void SetupPartyColumns()
        {
            // Create a horizontal stackpanel
            StackPanel stackPanel = new StackPanel
            {
                StackOrientation = Orientation.Horizontal
            };
            Children.Add(stackPanel);

            // Create a list of party views based on the party viewmodels.
            ColumnViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.PartyViewModels)
                .CreateDerivedListBinding(partyVM => new SequenceDiagramColumnView(this, partyVM))
                .ResultList;

            // Automatically add and remove columns to the stackpanel.
            ColumnViews.OnAdd.Subscribe(e => stackPanel.Children.Insert(e.Index, e.Element));
            ColumnViews.OnAdd.Subscribe(e =>
            {
                if (IsVisible && (IsFocused || HasChildInFocus()))
                {
                    e.Element.PartyView.LabelView.IsInEditMode = true;
                    e.Element.PartyView.LabelView.Focus();
                }
            });

            ColumnViews.OnDelete.Subscribe(e => stackPanel.Children.RemoveAt(e.Index));
        }

        private void SetupMessages()
        {
            // Create a list of message views based on the message viewmodels.
            _messageViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.StackVM.MessageViewModels)
                .CreateDerivedListBinding(vm => new SequenceDiagramMessageView {ViewModel = vm}).ResultList;

            // Automatically add and remove message views to Children.
            _messageViews.OnAdd.Subscribe(e => Children.Add(e.Element));
            _messageViews.OnDelete.Subscribe(e => Children.Remove(e.Element));
        }

        private void SetupPendingMessage()
        {
            // Hide the pending message view if there is no pending message.
            ViewModelChanged
                .ObserveNested(vm => vm.StackVM.PendingInvokingMessageVMChanged)
                .Select(vm => vm != null)
                .Subscribe(hasPendingMessage => _pendingMessageView.IsVisible = hasPendingMessage);

            // Set the correct start and endpoint for the pending message view.
            ViewModelChanged
                .ObserveNested(vm => vm.StackVM.PendingInvokingMessageVMChanged)
                .ObserveNested(vm => vm.SenderActivationBarChanged)
                .Subscribe(sender =>
                {
                    // Find the senders lifeline view.
                    var lifeLine = ColumnViews
                        .First(c => c.PartyView.ViewModel.Party == sender.Party)
                        .LifeLineView;

                    // Get the correct activation bar, if any.
                    var bar = lifeLine.ActivationBarViews.FirstOrDefault(b => b.ViewModel == sender);

                    if (bar == null)
                    {
                        // Get the startpoint of the pending message arrow, relative to the lifeline.
                        Point pointOnLifeline = new Point(
                            lifeLine.Width / 2,
                            (LifeLineView.TickHeight * ViewModel.StackVM.PendingInvokingMessageVM.Tick) -
                            (LifeLineView.TickHeight / 2)
                        );

                        // Translate the point to this view, and assign it.
                        _pendingMessageView.StartPoint = lifeLine.TranslatePointTo(this, pointOnLifeline);
                    }
                    else
                    {
                        // Calculate how far into this activation the pending message is sent.
                        int relativeTick = ViewModel.StackVM.PendingInvokingMessageVM.Tick -
                                           (bar?.ViewModel.StartTick ?? 0);

                        // Get the startpoint of the pending message arrow, relative to the activation bar.
                        Point pointOnBar = new Point(
                            bar.Width,
                            (LifeLineView.TickHeight * relativeTick) - (LifeLineView.TickHeight / 2)
                        );

                        // Translate the point to this view, and assign it.
                        _pendingMessageView.StartPoint = bar.TranslatePointTo(this, pointOnBar);
                    }
                });

            Children.Add(_pendingMessageView);
        }

        /// <see cref="OnMouseEvent"/>
        protected override void OnMouseEvent(MouseEventData e)
        {
            // Add a new party on double click
            if (e.Id == MouseEvent.MOUSE_CLICKED && e.ClickCount % 2 == 0)
            {
                //Add a new Party.
                ViewModel.AddParty(e.MousePosition);
                e.IsHandled = true;
                return;
            }

            // Update the endpoint position of the pending message when the mouse is dragged around the view.
            if (e.Id == MouseEvent.MOUSE_DRAGGED && ViewModel?.StackVM.PendingInvokingMessageVM != null)
            {
                _pendingMessageView.EndPoint = new Point(e.MousePosition.X, _pendingMessageView.StartPoint.Y);
                e.IsHandled = true;
                return;
            }

            base.OnMouseEvent(e);
        }
    }
}