using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;

namespace Interactr.View
{
    public class SequenceDiagramView : UIElement
    {
        #region ViewModel
        private readonly ReactiveProperty<SequenceDiagramViewModel> _viewModel = new ReactiveProperty<SequenceDiagramViewModel>();
        public SequenceDiagramViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }
        public IObservable<SequenceDiagramViewModel> ViewModelChanged => _viewModel.Changed;
        #endregion

        public SequenceDiagramView()
        {
            //The visibility of this view is set by the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged).Subscribe(isVisible =>
            {
                this.IsVisible = isVisible;
            });
        }
    }
}
