using System;
using System.Linq;
using System.Windows.Forms;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Dialogs;
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
        }

        /// <see cref="OnKeyEvent"/>
        protected override void OnKeyEvent(KeyEventData eventData)
        {
            if (eventData.Id == KeyEvent.KEY_PRESSED)
            {
                // Switch views on tab.
                if (eventData.KeyCode == KeyEvent.VK_TAB)
                {
                    ViewModel?.SwitchViews();

                    // Cancel the event propagation.
                    eventData.IsHandled = true;
                }
                // Show diagram dialog on CTRL+Enter
                else if (Keyboard.IsKeyDown(KeyEvent.VK_CONTROL) && eventData.KeyCode == (int)Keys.Enter)
                {
                    var dialogVM = ViewModel.CreateDiagramDialogViewModel();
                    var window = Dialog.OpenDialog(this, new DiagramDialogView { ViewModel = dialogVM }, "Diagram options", 200, 100);
                }
            }
        }
    }
}