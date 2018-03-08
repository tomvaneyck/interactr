using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
            // Define the visibility of this view to be set to the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged)
                .Subscribe(isVisible => { this.IsVisible = isVisible; });

            // Create a list of party views based on the party viewmodel.
            ReactiveList<PartyView> partyViews = ViewModelChanged.Select(vm => vm.PartyViewModels)
                .CreateDerivedListBinding(vm => new PartyView { ViewModel = vm }).ResultList;
            // Automatically add and remove party views to Children.
            partyViews.OnAdd.Subscribe(e => Children.Add(e.Element));
            partyViews.OnDelete.Subscribe(e => Children.Remove(e.Element));
        }
    }
}