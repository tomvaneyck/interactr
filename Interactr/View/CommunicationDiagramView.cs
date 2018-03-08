using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.ViewModel;

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

        public CommunicationDiagramView()
        {
            //Define the visibility of this view to be set to the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged)
                .Subscribe(isVisible => { this.IsVisible = isVisible; });

            // Define that a new party view gets added to this view when a new partyViewModel is added in the viewmodel.
            ViewModelChanged.ObserveNested(vm => vm.PartyViewModelOnAdd).Subscribe(partyViewModel =>
            {
                Children.Add(new PartyView() {ViewModel = partyViewModel});
            });

            // Define that all party views corresponding with the party view model get deleted when 
            // the party view model is deleted.
            ViewModelChanged.ObserveNested(vm => vm.PartyViewModelOnDelete).Subscribe(partyViewModel =>
            {
                foreach (var partyView in Children.OfType<PartyView>())
                {
                    if (partyView.ViewModel == partyViewModel)
                    {
                        Children.Remove(partyView);
                    }
                }
            });
        }
    }
}