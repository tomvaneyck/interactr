using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.ViewModel.Dialogs;

namespace Interactr.ViewModel
{
    /// <summary>
    /// A container for the label of an invocation message, also containing
    /// the methodName and methodArguments that get parsed out of the text.
    /// </summary>
    public class InvocationFormatStringViewModel : IFormatStringViewModel
    {
        #region Text

        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        /// <see cref="Text"/>
        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        public IObservable<string> TextChanged => _text.Changed;

        #endregion

        #region MethodName

        private readonly ReactiveProperty<string> _methodName = new ReactiveProperty<string>();

        public string MethodName
        {
            get => _methodName.Value;
            set => _methodName.Value = value;
        }

        public IObservable<string> MethodNameChanged => _methodName.Changed;

        #endregion

        public ReactiveList<string> MethodArguments { get; } = new ReactiveArrayList<string>();

        private bool _isUpdating = false;

        public InvocationFormatStringViewModel()
        {
            // Update the methodName and the method arguments when the label in the viewmodel changes.
            TextChanged.Subscribe(newLabelText =>
            {
                if (!_isUpdating)
                {
                    _isUpdating = true;

                    UpdateMethodPropertiesFromLabel();

                    _isUpdating = false;
                }
            });

            // Update the label on a change in the methodName or methodArguments.
            var methodArgumentsChanged = ReactiveExtensions.MergeEvents(
                MethodArguments.OnAdd,
                MethodArguments.OnDelete,
                MethodArguments.OnMoved.Where(c => c.Reason == MoveReason.Reordering)
            );
            MethodNameChanged.MergeEvents(methodArgumentsChanged).Subscribe(_ =>
            {
                if (!_isUpdating)
                {
                    _isUpdating = true;

                    UpdateLabelFromMethodProperties();

                    _isUpdating = false;
                }
            });
        }

        /// <summary>
        /// Indicate wether the text of this  invocation messageLabel is a valid label.
        /// </summary>
        /// <returns></returns>
        public bool HasValidText()
        {
            return Message.IsValidInvocationLabel(Text);
        }

        private void UpdateMethodPropertiesFromLabel()
        {
            var newMethodName = InvocationLabelParser.RetrieveMethodNameFromLabel(Text);
            var newMethodArguments = InvocationLabelParser.RetrieveArgumentsFromLabel(Text);

            MethodName = newMethodName;
            if (newMethodArguments != null)
            {
                MethodArguments.Clear();
                MethodArguments.AddRange(newMethodArguments);
            }
        }

        private void UpdateLabelFromMethodProperties()
        {
            Text = $"{MethodName}({String.Join(",", MethodArguments)})";
        }

        public InvocationMessageDiagramViewModel CreateNewDialogViewModel(Message model)
        {
            var dialogVM = new InvocationMessageDiagramViewModel();
            model.LabelChanged.Subscribe(newLabel => dialogVM.Message.Text = newLabel);
            dialogVM.Message.TextChanged
                .Where(_ => dialogVM.Message.HasValidText())
                .Subscribe(newLabel => model.Label = newLabel);

            return dialogVM;
        }
    }
}