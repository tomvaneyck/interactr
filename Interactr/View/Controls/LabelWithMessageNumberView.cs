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
        public LabelView LabelView { get; } = new LabelView();

        public LabelView MessageNumberView { get; } = new LabelView();

        public string WholeText
        {
            get => MessageNumber + LabelView.Text;
        }

        #region MessageNumber 

        public string MessageNumber
        {
            get => MessageNumberView.Text;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    MessageNumberView.Text = value + ":";
                }
            }
        }

        #endregion

        public LabelWithMessageNumberView()
        {
            StackOrientation = Orientation.Horizontal;

            Children.Add(MessageNumberView);
            Children.Add(LabelView);

            CanBeFocused = false;

            MessageNumberView.IsReadOnly = true;

            LabelView.PreferredHeightChanged.Subscribe(h => LabelView.Height = h);
            LabelView.PreferredWidthChanged.Subscribe(h => LabelView.Width = h);

            // Bind the width of this LabelWithMessageNumberView to the width of messageNumber and labelView.
            LabelView.PreferredWidthChanged.MergeEvents(MessageNumberView.PreferredWidthChanged)
                .Subscribe(_ => { PreferredWidth = LabelView.PreferredWidth + MessageNumberView.PreferredWidth; });

            // Bind the height of this LabelWithMessageNumberView to the height of the labelView.
            LabelView.HeightChanged.Subscribe(h => { PreferredHeight = h; });

            // Paint on a change in messageNumber or label
            LabelView.TextChanged.MergeEvents(MessageNumberView.TextChanged).Subscribe(_ => Repaint());
        }
    }
}