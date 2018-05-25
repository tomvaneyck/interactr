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

            var methodNameTextBox = new TextBox();

            ViewModel.MethodNameChanged.Subscribe(newText => methodNameTextBox.Text = newText);
            methodNameTextBox.TextChanged.Subscribe(newText => ViewModel.MethodName = newText);

            // Anchor and margin properties.
            AnchorsProperty.SetValue(methodNameTextBox, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(methodNameTextBox, new Margins(5, 5));

            Children.Add(methodNameTextBox);

//            var methodArgsListBox = new ListBox<string>(e => new LabelView() {Text = e.Value});
//            
//            //Bind textbox and viewModel
//            ViewModel.MethodArguments.OnAdd.Subscribe(newText => methodArgsListBox.ItemsSource.Insert(newText.Index,newText.Element));
        }
    }
}