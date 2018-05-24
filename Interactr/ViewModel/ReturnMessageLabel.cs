using System;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class ReturnFormatStringViewModel : IFormatStringViewModel
    {
        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        /// <summary>
        /// The text of the Label stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the label in the message model.
        /// If the changes of viewModel are not propogated to the model for example.
        /// Any changes to the model are however immediately propagated to the viewmodel.
        /// </remarks>
        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        public IObservable<string> TextChanged => _text.Changed;

        public bool IsValidLabel()
        {
            return true;
        }
    }
}