using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public SequenceDiagramView()
        {
            // Define the visibility of this view to be set to the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged)
                .Subscribe(isVisible => { this.IsVisible = isVisible; });

            StackPanel stackPanel = new StackPanel
            {
                StackOrientation = Orientation.Horizontal
            };
            Children.Add(stackPanel);

            // Create a list of party views based on the party viewmodel.
            ReactiveList<PartyView> partyViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.PartyViewModels)
                .CreateDerivedListBinding(vm => new PartyView {ViewModel = vm})
                .ResultList;
            // Automatically add and remove party views to Children.
            partyViews.OnAdd.Subscribe(e => Children.Add(e.Element));
            partyViews.OnDelete.Subscribe(e => Children.Remove(e.Element));
        }

        protected override bool OnMouseEvent(MouseEventData e)
        {
            Debug.WriteLine(e.ClickCount);
            Debug.WriteLine("ID: " + e.Id);
            if (e.Id == MouseEvent.MOUSE_CLICKED)
            {
                Debug.WriteLine("Add Party.");
                //Add a new Party.
                ViewModel.AddParty();
                return true;
            }

            return false;
        }
    }
}