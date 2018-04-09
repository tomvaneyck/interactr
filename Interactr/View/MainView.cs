﻿using System;
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
            var diagramEditors = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.Diagrams)
                .CreateDerivedListBinding(diagramVm => new DiagramEditorView {ViewModel = diagramVm})
                .ResultList;

            //Add the views to the view tree.
            diagramEditors.OnAdd.Subscribe(e => Windows.Children.Add(new WindowsView.Window(e.Element)));
            diagramEditors.OnDelete.Subscribe(e => Windows.Children.Remove(Windows.Children.OfType<WindowsView.Window>().First(w => w.InnerElement == e.Element)));

            this.Children.Add(Windows);
        }
    }
}