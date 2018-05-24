using System;

namespace Interactr.ViewModel
{
    /// <summary>
    /// An interface for a container for text.
    /// If additional formatting takes place it can additionally stored in this container.
    /// </summary>
    public interface IFormatStringViewModel
    {
        /// <summary>
        /// Return true if the label is valid.
        /// </summary>
        /// <returns></returns>
        bool HasValidText();

        /// <summary>
        /// The text stored.
        /// </summary>
        string Text { get; set; }

        IObservable<string> TextChanged { get; }
    }
}