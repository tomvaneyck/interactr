using System;
using Interactr.Reactive;
using Interactr.View.Controls;
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
    }
}