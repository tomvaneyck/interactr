using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    public class DiagramEditorView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<DiagramEditorViewModel> _viewModel = new ReactiveProperty<DiagramEditorViewModel>();

        public DiagramEditorViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<DiagramEditorViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public CommunicationDiagramView CommDiagView { get; } = new CommunicationDiagramView();
        public SequenceDiagramView SeqDiagView { get; } = new SequenceDiagramView();

        public DiagramEditorView()
        {
            ViewModelChanged.Subscribe(viewModel =>
            {
                //Retrieve the viewmodels for the child views from this elements viewmodel
                CommDiagView.ViewModel = viewModel?.CommDiagramVM;
                SeqDiagView.ViewModel = viewModel?.SeqDiagramVM;
            });

            //Add the views to the view tree.
            this.Children.Add(CommDiagView);
            this.Children.Add(SeqDiagView);

            this.PreferredWidth = 500;
            this.PreferredHeight = 500;
        }

        /// <see cref="OnKeyEvent"/>
        protected override bool OnKeyEvent(KeyEventData e)
        {
            if (e.Id == KeyEvent.KEY_PRESSED && e.KeyCode == KeyEvent.VK_TAB)
            {
                ViewModel?.SwitchViews();
                return true;
            }

            return false;
        }
    }
}
