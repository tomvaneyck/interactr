using System;
using System.Reactive.Linq;
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

        private readonly ListBox<string> _methodArgumentsListBox;

        public InvocationMessageDialogView(InvocationMessageDiagramViewModel viewModel)
        {
            ViewModel = viewModel;

            // Method name label
            LabelView methodNameLabel = new LabelView
            {
                Text = "Method name:",
                IsVisibleToMouse = false,
                IsReadOnly = true,
                CanBeFocused = false
            };
            AnchorsProperty.SetValue(methodNameLabel, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(methodNameLabel, new Margins(5, 5));
            this.Children.Add(methodNameLabel);

            // Method name textbox
            TextBox methodNameTextBox = new TextBox();
            ViewModel.Message.MethodNameChanged.Subscribe(newText => methodNameTextBox.Text = newText);
            methodNameTextBox.TextChanged.Subscribe(newText => ViewModel.Message.MethodName = newText);
            AnchorsProperty.SetValue(methodNameTextBox, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(methodNameTextBox, new Margins(140, 5));
            this.Children.Add(methodNameTextBox);

            // Method arguments label
            LabelView methodArgumentsLabel = new LabelView
            {
                Text = "Method arguments:",
                IsVisibleToMouse = false,
                IsReadOnly = true,
                CanBeFocused = false
            };
            AnchorsProperty.SetValue(methodArgumentsLabel, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(methodArgumentsLabel, new Margins(5, 30));
            this.Children.Add(methodArgumentsLabel);

            // Method arguments listbox
            _methodArgumentsListBox = new ListBox<string>(e =>
            {
                var labelView = new LabelView {Text = e};
                labelView.EditModeChanged
                    .Where(editMode => !editMode)
                    .Where(_ => labelView.Parent != null)
                    .Subscribe(newLabel =>
                    {
                        int i = _methodArgumentsListBox.ListView.GetSourceIndexOfView(labelView);
                        _methodArgumentsListBox.ItemsSource[i] = labelView.Text;
                    });
                return labelView;
            });
            _methodArgumentsListBox.ItemsSource = viewModel.Message.MethodArguments;

            // Anchor and margin properties.
            AnchorsProperty.SetValue(_methodArgumentsListBox, Anchors.Left | Anchors.Top | Anchors.Bottom);
            MarginsProperty.SetValue(_methodArgumentsListBox, new Margins(140, 30, 0, 5));
            
            Children.Add(_methodArgumentsListBox);
        }
    }
}