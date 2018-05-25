using System;
using Interactr.Reactive;

namespace Interactr.ViewModel.Dialogs
{
    public class InvocationMessageDiagramViewModel
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

    }
}