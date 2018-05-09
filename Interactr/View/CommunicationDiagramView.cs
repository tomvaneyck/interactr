using System;
using System.ComponentModel;
using System.Reactive;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Interactr.Constants;
using Interactr.Reactive;
using Interactr.View;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    /// <summary>
    /// The view for the communication diagram.
    /// </summary>
    public class CommunicationDiagramView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<CommunicationDiagramViewModel> _viewModel =
            new ReactiveProperty<CommunicationDiagramViewModel>();

        public CommunicationDiagramViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<CommunicationDiagramViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public readonly PartyViewsDragPanel PartyViewsDragPanel;

        private readonly IReadOnlyReactiveList<CommunicationDiagramMessageView> _messageViews;
        
        private IReadOnlyReactiveList<CommunicationDiagramPartyView> _partyViews;
        
        public CommunicationDiagramView()
        {
            // Define the visibility of this view to be set to the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged)
                .Subscribe(isVisible => { this.IsVisible = isVisible; });

            // Create a list of party views based on the party viewmodel.
            _partyViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.PartyViewModels)
                .CreateDerivedListBinding(vm => new CommunicationDiagramPartyView() {ViewModel = vm})
                .ResultList;

            // Create the partyviews drag panel.
            PartyViewsDragPanel = new PartyViewsDragPanel(_partyViews);
            Children.Add(PartyViewsDragPanel);

            // Create a list of message views based on the message viewmodels.
            _messageViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.InvocationMessageViewModels)
                .CreateDerivedListBinding(vm => new CommunicationDiagramMessageView(vm)).ResultList;

            // Automatically add and remove message views to Children.
            _messageViews.OnAdd.Subscribe(e =>
            {
                // Make message views the size of the communication diagram view.
                e.Element.PreferredWidth = Width;
                e.Element.PreferredHeight = Height;

                // Assing arrow start and end points on the message.
                AssignAnchorPointsToMessage(e.Element);

                Children.Add(e.Element);
            });

            // Remove a message view from the children when deleted.
            _messageViews.OnDelete.Subscribe(e => Children.Remove(e.Element));
        }

        /// <see cref="OnMouseEvent"/>
        protected override bool OnMouseEvent(MouseEventData e)
        {
            // Add a new party on double click
            if (e.Id == MouseEvent.MOUSE_CLICKED && e.ClickCount % 2 == 0 && FocusedElement.CanLoseFocus)
            {
                Debug.WriteLine("Add Party.");
                //Add a new Party.
                ViewModel.AddParty(e.MousePosition);
                return true;
            }
            else
            {
                return base.OnMouseEvent(e);
            }
        }

        /// <see cref="OnKeyEvent"/>
        protected override bool OnKeyEvent(KeyEventData eventData)
        {
            // Delete party.
            // The commented check is an extra safety, but not yet possible due
            // to the need of a recursive search.
            if (eventData.Id == KeyEvent.KEY_RELEASED &&
                eventData.KeyCode == KeyCodes.Delete &&
                /*Children.Contains(FocusedElement) &&*/
                FocusedElement.GetType() == typeof(LabelView)
            )
            {
                var elementToDelete = FocusedElement.Parent;
                if (elementToDelete is PartyView)
                {
                    // Delete the party from the viewmodel. This automatically
                    // propagates to the view and the model.
                    ViewModel.DeleteParty(((PartyView) elementToDelete).ViewModel.Party);
                    return true;
                }
                else if (elementToDelete is CommunicationDiagramMessageView)
                {
                    // Delete the message from the viewmodel. This automatically propagates
                    // to the view and the model.
                    ViewModel.DeleteMessage(((CommunicationDiagramMessageView) elementToDelete).ViewModel.Message);
                }
            }

            return false;
        }

        /// <summary>
        /// Assign two anchor points to a messageview.
        /// The arrow startpoint of the message gets connected to an arrowAnchorElement on an arrowStack of the sender.
        /// The arrow endpoint of the message gets connected to an arrowAnchorElement on an arrowStack of the receiver.
        /// This provides automatic placement  and ordering of the message arrows between a sender and a receiver.
        /// </summary>
        /// <param name="messageView"> The message view to assign anchors to</param>
        private void AssignAnchorPointsToMessage(CommunicationDiagramMessageView messageView)
        {
            // Retrieve the sender and receiver party views.
            CommunicationDiagramPartyView senderPartyView =
                PartyViewsDragPanel.PartyViews.First(pv => pv.ViewModel.Party == messageView.ViewModel.Message.Sender);

            CommunicationDiagramPartyView receiverPartyView =
                PartyViewsDragPanel.PartyViews.First(pv =>
                    pv.ViewModel.Party == messageView.ViewModel.Message.Receiver);

            CommunicationDiagramPartyView.ArrowAnchor senderAnchor;
            CommunicationDiagramPartyView.ArrowAnchor receiverAnchor;

            var anchorWidth = 3;
            var anchorHeight = 17;

            // Choose the arrowStack to attach the arrowAnchorElements to, 
            // and attach them to this arrowStack. This happens for both the sender
            // and the receiver.
            if (receiverPartyView.Position.X < senderPartyView.Position.X)
            {
                senderAnchor =
                    senderPartyView.LeftArrowStack.AddArrowAnchorElement(anchorWidth, anchorHeight);
                receiverAnchor =
                    receiverPartyView.RightArrowStack.AddArrowAnchorElement(anchorWidth, anchorHeight);
            }
            else
            {
                senderAnchor = senderPartyView.RightArrowStack.AddArrowAnchorElement(anchorWidth, anchorHeight);
                receiverAnchor = receiverPartyView.LeftArrowStack.AddArrowAnchorElement(anchorWidth, anchorHeight);
            }

            // Anchor to the sender.
            senderAnchor.AbsolutePositionChanged.Subscribe(newPos =>
                messageView.ArrowStartPoint = newPos - (Parent?.Position ?? new Point(0, 0)) -
                                              (Parent?.Parent?.Position ?? new Point(0, 0)));

            // Anchor to the Receiver
            receiverAnchor.AbsolutePositionChanged.Subscribe(newPos =>
                messageView.ArrowEndPoint = newPos - (Parent?.Position ?? new Point(0, 0)) -
                                            (Parent?.Parent?.Position ?? new Point(0, 0)));

            // Dynamically change if the message is achored to the left or right arrowStack in the partyview.
            receiverPartyView.PositionChanged.Merge(senderPartyView.PositionChanged).Subscribe(
                _ =>
                {
                    // If the receiver is to the left of the sender and the sender has an arrow starting on it's rightArrowStack.
                    if (receiverPartyView.Position.X < senderPartyView.Position.X &&
                        senderPartyView.RightArrowStack.Children.Contains(senderAnchor) &&
                        receiverPartyView.LeftArrowStack.Children.Contains(receiverAnchor))
                    {
                        // Switch the anchors from a left to a right stack or vice versa.
                        senderPartyView.RightArrowStack.Children.Remove(senderAnchor);
                        receiverPartyView.LeftArrowStack.Children.Remove(receiverAnchor);

                        senderPartyView.LeftArrowStack.Children.Add(senderAnchor);
                        receiverPartyView.RightArrowStack.Children.Add(receiverAnchor);
                    }
                    // If the receiver is to the right of the  sender and the sender has an arrow starting on it's leftArrowStack.
                    else if (receiverPartyView.Position.X > senderPartyView.Position.X &&
                             senderPartyView.LeftArrowStack.Children.Contains(senderAnchor) &&
                             receiverPartyView.RightArrowStack.Children.Contains(receiverAnchor))
                    {
                        // Switch the anchors from a left to a right stack or vice versa.
                        senderPartyView.LeftArrowStack.Children.Remove(senderAnchor);
                        receiverPartyView.RightArrowStack.Children.Remove(receiverAnchor);

                        senderPartyView.RightArrowStack.Children.Add(senderAnchor);
                        receiverPartyView.LeftArrowStack.Children.Add(receiverAnchor);
                    }
                }
            );

            // Delete the anchors when the message gets deleted.
            _messageViews.OnDelete.Where(mv => mv.Element == messageView)
                .Subscribe(_ =>
                {
                    senderPartyView.RightArrowStack.Children.Remove(senderAnchor);
                    receiverPartyView.LeftArrowStack.Children.Remove(receiverAnchor);
                });
        }
    }

    /// <summary>
    /// A drag panel to enable dragging of the parties in Communication diagram view.
    /// </summary>
    public class PartyViewsDragPanel : DragPanel
    {
        public readonly IReadOnlyReactiveList<CommunicationDiagramPartyView> PartyViews;

        public PartyViewsDragPanel(IReadOnlyReactiveList<CommunicationDiagramPartyView> partyViews)
        {
            {
                PartyViews = partyViews;

                // Automatically enter label editing mode when adding a party
                PartyViews.OnAdd.Subscribe(elem =>
                {
                    if (IsVisible && (IsFocused || HasChildInFocus()))
                    {
                        elem.Element.LabelView.IsInEditMode = true;
                        elem.Element.LabelView.Focus();
                    }
                });

                // Two-way binding between the viewmodel and view position.
                partyViews.ObserveEach(partyView => partyView.ViewModel.PositionChanged)
                    .Subscribe(e => e.Element.Position = e.Value);
                partyViews.ObserveEach(partyView => partyView.PositionChanged)
                    .Subscribe(e => e.Element.ViewModel.Position = e.Value);

                // Automatically add and remove party views to Children.
                PartyViews.OnAdd.Subscribe(e => Children.Add(e.Element));
                PartyViews.OnDelete.Subscribe(e => Children.Remove(e.Element));
            }
        }
    }
}