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
    /// This element places a messageNumber view than cannot be focused nor edited in front of the labelView.
    /// The LabelWithMessageNumberView cannot be 
    /// </summary>
    public class LabelWithMessageNumberView : UIElement
    {
        public LabelView LabelView { get; } = new LabelView();

        public MessageNumberViewClass MessageNumberView { get; } = new MessageNumberViewClass();

        public LabelWithMessageNumberView()
        {
            Children.Add(LabelView);
            Children.Add(MessageNumberView);

            CanBeFocused = false;

            // Change the position of the messageNumber view if the position of the labelView changes.
            MessageNumberView.WidthChanged.Subscribe(w =>
            {
                LabelView.Position = new Point(MessageNumberView.Position.X + w, MessageNumberView.Position.Y);
            });

            // Bind the width of this LabelWithMessageNumberView to the width of messageNumber and labelView.
            LabelView.WidthChanged.MergeEvents(MessageNumberView.WidthChanged)
                .Subscribe(_ => { Width = LabelView.Width + MessageNumberView.Width;
                    PreferredWidth = Width;
                });

            // Bind the height of this LabelWithMessageNumberView to the height of the labelView.
            LabelView.HeightChanged.Subscribe(h => {
                Height = h;
                PreferredHeight = h;
            });

            // Paint on a change in messageNumber or label
            LabelView.TextChanged.MergeEvents(MessageNumberView.MessageNumberChanged).Subscribe(_ => Repaint());
        }

        /// <summary>
        /// A LabelView that cannot be focused and is not visible to mouse. 
        /// This View contains the messageNumber.
        /// </summary>
        public class MessageNumberViewClass : LabelView
        {
            #region MessageNumber 

            private readonly ReactiveProperty<string> _messageNumber = new ReactiveProperty<string>();

            public string MessageNumber
            {
                get => _messageNumber.Value;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _messageNumber.Value = value + ":";
                        Text = _messageNumber.Value;
                    }
                }
            }

            public IObservable<string> MessageNumberChanged => _messageNumber.Changed;

            #endregion

            public MessageNumberViewClass()
            {
                // This element can never be focused or receive mouse events.
                CanBeFocused = false;
                IsVisibleToMouse = false;
            }
        }
    }
}