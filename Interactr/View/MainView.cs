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
using Interactr.Window;

namespace Interactr.View
{
    /// <summary>
    /// The top-level view.
    /// </summary>
    public class MainView : AnchorPanel
    {
        public WindowsView Windows { get; } = new WindowsView();
        
        #region ViewModel

        private readonly ReactiveProperty<MainViewModel> _viewModel = new ReactiveProperty<MainViewModel>();

        public MainViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<MainViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public MainView()
        {
            // Create DiagramEditorViews for each Diagram
            var diagramEditors = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.DiagramEditors)
                .CreateDerivedListBinding(diagramVm => new DiagramEditorView {ViewModel = diagramVm})
                .ResultList;

            // Add the diagram views to the view tree.
            diagramEditors.OnAdd.Subscribe(e => Windows.AddWindow(e.Element));
            diagramEditors.OnDelete.Subscribe(e => Windows.RemoveWindowWith(e.Element));

            // Remove a diagram editor when its window closes.
            Windows.WindowCloseRequested.Subscribe(window =>
            {
                if (window.InnerElement is DiagramEditorView editor)
                {
                    ViewModel.CloseEditor(editor.ViewModel);
                }
            });

            this.Children.Add(Windows);
        }

        protected override bool OnKeyEvent(KeyEventData eventData)
        {
            // Create a new diagram when the user presses CTRL+N
            if (eventData.Id == KeyEvent.KEY_PRESSED && eventData.KeyCode == KeyEvent.VK_N && Keyboard.IsKeyDown(KeyEvent.VK_CONTROL))
            {
                ViewModel.EditNewDiagram();
                return true;
            }

            return false;
        }
    }
}