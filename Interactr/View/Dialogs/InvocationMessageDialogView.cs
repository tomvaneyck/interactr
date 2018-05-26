using System;
using System.Runtime.Remoting.Messaging;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.ViewModel.Dialogs;

namespace Interactr.View.Dialogs
{
    public class InvocationMessageDialogView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<InvocationMessageDiagramViewModel> _viewModel =
            new ReactiveProperty<InvocationMessageDiagramViewModel>();

        public InvocationMessageDiagramViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<InvocationMessageDiagramViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public InvocationMessageDialogView(InvocationMessageDiagramViewModel viewModel)
        {
            ViewModel = viewModel;

            // Method name label
            LabelView methodNameLabel = new LabelView
            {
                Text = "Method name:",
                IsVisibleToMouse = false,
                IsReadOnly = true
            };
            AnchorsProperty.SetValue(methodNameLabel, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(methodNameLabel, new Margins(5, 5));
            this.Children.Add(methodNameLabel);

            // Method name textbox
            TextBox methodNameTextBox = new TextBox();
            ViewModel.MethodNameChanged.Subscribe(newText => methodNameTextBox.Text = newText);
            methodNameTextBox.TextChanged.Subscribe(newText => ViewModel.MethodName = newText);
            AnchorsProperty.SetValue(methodNameTextBox, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(methodNameTextBox, new Margins(140, 5));
            this.Children.Add(methodNameTextBox);

            // Method arguments label
            LabelView methodArgumentsLabel = new LabelView
            {
                Text = "Method arguments:",
                IsVisibleToMouse = false,
                IsReadOnly = true
            };
            AnchorsProperty.SetValue(methodArgumentsLabel, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(methodArgumentsLabel, new Margins(5, 30));
            this.Children.Add(methodArgumentsLabel);

            // Method arguments listbox
            var methodArgsListBox = new ListBox<string>(e => new LabelView {Text = e})
            {
                ItemsSource = viewModel.MethodArguments
            };

            // Anchor and margin properties.
            AnchorsProperty.SetValue(methodArgsListBox, Anchors.Left | Anchors.Top | Anchors.Bottom);
            MarginsProperty.SetValue(methodArgsListBox, new Margins(140, 30, 0, 5));
            
            Children.Add(methodArgsListBox);
        }
    }
}