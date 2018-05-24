using System;
using System.Collections.Generic;
using System.Net.Mime;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class PartyFormatStringViewModel : IFormatStringViewModel
    {
        #region Text

        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        /// <see cref="MediaTypeNames.Text"/>
        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        public IObservable<string> TextChanged => _text.Changed;

        #endregion

        #region MethodName

        private readonly ReactiveProperty<string> _instanceName = new ReactiveProperty<string>();

        private string InstanceName
        {
            get => _instanceName.Value;
            set => _instanceName.Value = value;
        }

        #endregion

        #region ClassName

        private readonly ReactiveProperty<List<string>> _className = new ReactiveProperty<List<string>>();

        private List<string> ClassName
        {
            get => _className.Value;
            set => _className.Value = value;
        }

        #endregion

        public bool IsValidLabel()
        {
            return Party.IsValidLabel(Text);
        }
    }
}