using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.View.Controls;
using Interactr.View.Framework;

namespace Interactr.View.Dialogs
{
    public static class Dialog
    {
        /// <summary>
        /// Open a dialog window.
        /// </summary>
        /// <remarks>
        /// Windows opened with this method will automatically closed when the close button is pressed.
        /// If there is no WindowView ancestor to add the dialog window to, this method returns null.
        /// </remarks>
        /// <param name="origin">The element that will be used to find a WindowsView ancestor.</param>
        /// <param name="dialog">The contents of the dialog.</param>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="width">The width of the dialog window.</param>
        /// <param name="height">The height of the dialog window.</param>
        /// <returns>The dialog window, or null if not WindowsView was found.</returns>
        public static WindowsView.Window OpenDialog(UIElement origin, UIElement dialog, string title = "New Dialog", int width = 500, int height = 500)
        {
            // Find a WindowsView.
            var windowsView = origin.WalkToRoot().OfType<WindowsView>().FirstOrDefault();
            if (windowsView == null)
            {
                return null;
            }

            // Add the window and set the window properties.
            var window = windowsView.AddWindow(dialog, width, height);
            window.Title = title;

            // Close the dialog when the close button is pressed.
            var onDialogCloseRequested = windowsView.WindowCloseRequested.Where(w => w.InnerElement == dialog);
            var onDialogClosed = windowsView.Children.OnDelete.Where(e => ((WindowsView.Window)e.Element).InnerElement == dialog);

            onDialogCloseRequested.TakeUntil(onDialogClosed).Subscribe(_ =>
            {
                windowsView.RemoveWindowWith(dialog);
            });

            return window;
        }
    }
}
