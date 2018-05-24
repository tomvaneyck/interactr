using System;

namespace Interactr.ViewModel
{
    public interface IFormatStringViewModel
    {
        /// <summary>
        /// Return true if the label is valid.
        /// </summary>
        /// <returns></returns>
        bool HasValidLabel();

        /// <summary>
        /// The text of the Label stored in message view model.
        /// </summary>
        /// <remarks>This should not necessarily be the same as the label in the message model.
        /// If the changes of viewModel are not propogated to the model for example.
        /// Any changes to the model are however immediately propagated to the viewmodel.
        /// </remarks>
        string Text { get; set; }

        IObservable<string> TextChanged { get; }
    }
}