using System;

namespace Interactr.ViewModel
{
    public interface ILabelVM
    {
        /// <summary>
        /// Return true if the label is valid.
        /// </summary>
        /// <returns></returns>
        bool IsValidLabel();

        string Label { get; set; }

        IObservable<string> LabelChanged { get; }

        //DialogVM CreateDialogViewModel();
    }
}