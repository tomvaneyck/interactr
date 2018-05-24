using System;
using System.Collections.Generic;
using System.Linq;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class InvocationMessageLabelVM : ILabelVM
    {
        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

        /// <summary>
        /// The text of the Label stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the label in the message model.
        /// If the changes of viewModel are not propogated to the model for example.
        /// Any changes to the model are however immediately propagated to the viewmodel.
        /// </remarks>
        public string Label
        {
            get => _label.Value;
            set => _label.Value = value;
        }

        public IObservable<string> LabelChanged => _label.Changed;

        public bool IsValidLabel()
        {
            throw new System.NotImplementedException();
        }

        private readonly ReactiveProperty<string> _methodName = new ReactiveProperty<string>();

        public string MethodName
        {
            get => _methodName.Value;
            private set => _methodName.Value = value;
        }

        public IObservable<string> MethodNameChanged => _methodName.Changed;

        private readonly ReactiveProperty<List<string>> _methodArguments = new ReactiveProperty<List<string>>();

        public List<string> MethodArguments
        {
            get => _methodArguments.Value;
            private set => _methodArguments.Value = value;
        }

        public IObservable<IList<string>> MethodArgumentsChanged => _methodArguments.Changed;

        public InvocationMessageLabelVM()
        {
            // Update the methodName and the method arguments when the label in the viewmodel changes.
            LabelChanged.Subscribe(newLabelText =>
                {
                    var newMethodName = InvocationLabelParser.RetrieveMethodNameFromLabel(newLabelText);
                    var newMethodArguments = InvocationLabelParser.RetrieveArgumentsFromLabel(newLabelText);

                    MethodName = newMethodName;
                    if (newMethodArguments != null)
                    {
                        MethodArguments = newMethodArguments.ToList();
                    }
                }
            );
        }
    }
}