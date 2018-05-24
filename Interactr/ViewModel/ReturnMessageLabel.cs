using System;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class ReturnMessageLabelVM : ILabelVM
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
    }
}