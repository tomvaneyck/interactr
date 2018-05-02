using System;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    /// <summary>
    /// This view is used to display and edit diagrams.
    /// </summary>
    public class DiagramEditorView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<DiagramEditorViewModel> _viewModel =
            new ReactiveProperty<DiagramEditorViewModel>();

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
                // Retrieve the viewmodels for the child views from this elements viewmodel.
                CommDiagView.ViewModel = viewModel?.CommDiagramVM;
                SeqDiagView.ViewModel = viewModel?.SeqDiagramVM;
            });

            // Add the views to the view tree.
            this.Children.Add(CommDiagView);
            this.Children.Add(SeqDiagView);

            //TODO: this is needed to have working windows. Should be removed once window resizing is implemented.
            this.PreferredWidth = 500;
            this.PreferredHeight = 500;
        }

        /// <see cref="OnKeyEvent"/>
        protected override void OnKeyEvent(KeyEventData eventData)
        {
            // Switch views on tab.
            if (eventData.Id == KeyEvent.KEY_PRESSED && eventData.KeyCode == KeyEvent.VK_TAB)
            {
                ViewModel?.SwitchViews();

                // Cancel the event propagation.
                eventData.IsCancelled = true;
            }
        }
    }
}