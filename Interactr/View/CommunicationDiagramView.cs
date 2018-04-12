using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class CommunicationDiagramView : DragPanel
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

            // Automatically enter label editing mode when adding a party
            partyViews.OnAdd.Subscribe(elem =>
            {
                if (IsVisible && (IsFocused || HasChildInFocus()))
                {
                    elem.Element.LabelView.IsInEditMode = true;
                    elem.Element.LabelView.Focus();
                }
            });

            // Automatically add and remove party views to Children.
            partyViews.OnAdd.Subscribe(e => Children.Add(e.Element));
            partyViews.OnDelete.Subscribe(e => Children.Remove(e.Element));
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
}
