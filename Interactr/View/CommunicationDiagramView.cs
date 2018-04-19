using System;
using System.Diagnostics;
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

        public CommunicationDiagramView()
        {
            // Define the visibility of this view to be set to the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged)
                .Subscribe(isVisible => { this.IsVisible = isVisible; });

            // Create a list of party views based on the party viewmodel.
            IReadOnlyReactiveList<PartyView> partyViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.PartyViewModels)
                .CreateDerivedListBinding(vm => new PartyView {ViewModel = vm})
                .ResultList;

            // Create the partyviews drag panel.
            PartyViewsDragPanel = new PartyViewsDragPanel(partyViews);
            Children.Add(PartyViewsDragPanel);

            // Create a list of message views based on the message viewmodels.
            IReadOnlyReactiveList<CommunicationDiagramMessageView> messageViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.MessageViewModels)
                .CreateDerivedListBinding(vm => new CommunicationDiagramMessageView() {ViewModel = vm}).ResultList;

            // Automatically add and remove message views to Children.
            messageViews.OnAdd.Subscribe(e =>
            {
                // Make message views the size of the communication diagram view.
                e.Element.PreferredWidth = Width;
                e.Element.PreferredHeight = Height;

                Children.Add(e.Element);
            });
            messageViews.OnDelete.Subscribe(e => Children.Remove(e.Element));
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
                PartyView partyView = (PartyView) FocusedElement.Parent;

                // Delete the party from the viewmodel. This automatically
                // propagates to the view and the model.
                ViewModel.DeleteParty(partyView.ViewModel.Party);
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// A drag panel to enable dragging of the parties in Communication diagram view.
    /// </summary>
    public class PartyViewsDragPanel : DragPanel
    {
        public readonly IReadOnlyReactiveList<PartyView> PartyViews;

        public PartyViewsDragPanel(IReadOnlyReactiveList<PartyView> partyViews)
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