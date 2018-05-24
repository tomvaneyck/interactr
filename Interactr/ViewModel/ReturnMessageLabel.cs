using System;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class ReturnFormatStringViewModel : IFormatStringViewModel
    {
        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        /// <see cref="Text"/>
        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        public IObservable<string> TextChanged => _text.Changed;

        public bool HasValidText()
        {
            return true;
        }
    }
}