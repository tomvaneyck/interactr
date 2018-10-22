using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.ViewModel.Dialogs;

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

        public ReturnMessageDialogViewModel CreateNewDialogViewModel(Message model)
        {
            var dialogVM = new ReturnMessageDialogViewModel();
            
           //Bind the text with the dialogViewModel.
            TextChanged.Subscribe(newText =>
            {
                dialogVM.Text = newText;
                model.Label = newText;
            });
            dialogVM.TextChanged.Subscribe(newText => { Text = newText; });
            
            return dialogVM;
        }
    }
}