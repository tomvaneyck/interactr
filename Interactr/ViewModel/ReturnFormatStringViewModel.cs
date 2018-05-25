using System;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class ReturnFormatStringViewModel : IFormatStringViewModel
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

        public bool HasValidText()
        {
            return true;
        }
    }
}