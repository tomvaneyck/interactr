using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Interactr.Reactive;
using Interactr.View.Framework;
using Point = Interactr.View.Framework.Point;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A composite element containing a label and a messageNumberView. 
    /// This element places a messageNumber view that cannot be focused nor edited in front of the labelView.
    /// </summary>
    public class LabelWithMessageNumberView : StackPanel
    {
        public LabelView MessageNumberView { get; } = new LabelView();

        public LabelView LabelView { get; } = new LabelView();

        public string WholeText => MessageNumberView.Text + LabelView.Text;

        public LabelWithMessageNumberView()
        {
            StackOrientation = Orientation.Horizontal;

            Children.Add(MessageNumberView);
            Children.Add(LabelView);

            CanBeFocused = false;

            MessageNumberView.IsReadOnly = true;

            // Bind the width of this LabelWithMessageNumberView to the width of messageNumber and labelView.
            LabelView.PreferredWidthChanged.MergeEvents(MessageNumberView.PreferredWidthChanged)
                .Subscribe(_ => { PreferredWidth = LabelView.PreferredWidth + MessageNumberView.PreferredWidth; });

            // Bind the height of this LabelWithMessageNumberView to the height of the labelView.
            LabelView.PreferredHeightChanged.MergeEvents(MessageNumberView.PreferredHeightChanged)
                .Subscribe(h =>
                {
                    // Do not set the preferred height to zero
                    // When the labelView does not have elements and thus height zero 
                    // We still want the messageNumberView to be visible.
                    PreferredHeight = Math.Max(LabelView.PreferredHeight, MessageNumberView.PreferredHeight);
                });
        }

        public void SetMessageNumber(string value)
        {
            MessageNumberView.Text = string.IsNullOrEmpty(value) ? "" : value + ":";
        }
    }
}