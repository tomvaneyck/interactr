﻿using System;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using Interactr.Constants;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A view for displaying and editing the text label.
    /// </summary>
    public class LabelView : UIElement
    {
        #region Text

        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        /// <summary>
        /// The text of the label.
        /// </summary>
        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        /// <summary>
        /// Emit the new text when the text changes.
        /// </summary>
        public IObservable<string> TextChanged => _text.Changed;

        #endregion

        #region Font

        private readonly ReactiveProperty<Font> _font = new ReactiveProperty<Font>();

        /// <summary>
        /// The font for displaying the label text.
        /// </summary>
        public Font Font
        {
            get => _font.Value;
            set => _font.Value = value;
        }

        /// <summary>
        /// Emit the new Font when the font changes.
        /// </summary>
        public IObservable<Font> FontChanged => _font.Changed;

        #endregion

        #region Color

        private readonly ReactiveProperty<Color> _color = new ReactiveProperty<Color>();

        /// <summary>
        /// The color of the label text.
        /// </summary>
        public Color Color
        {
            get => _color.Value;
            set => _color.Value = value;
        }

        /// <summary>
        /// Emit the new Color when it changes.
        /// </summary>
        public IObservable<Color> ColorChanged => _color.Changed;

        #endregion

        #region IsInEditMode

        private readonly ReactiveProperty<bool> _isInEditMode = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if the label is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get => _isInEditMode.Value;
            set => _isInEditMode.Value = value;
        }

        /// <summary>
        /// Emit the new editMode when edit mode changes.
        /// </summary>
        public IObservable<bool> EditModeChanged => _isInEditMode.Changed;

        #endregion

        #region CanLeaveEditMode

        private readonly ReactiveProperty<bool> _canLeaveEditMode = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if leaving edit mode is allowed.
        /// </summary>
        public bool CanLeaveEditMode
        {
            get => _canLeaveEditMode.Value;
            set => _canLeaveEditMode.Value = value;
        }

        /// <summary>
        /// Emit the new canLeaceEditMode value when it changes.
        /// </summary>
        public IObservable<bool> CanLeaveEditModeChanged => _canLeaveEditMode.Changed;

        #endregion

        #region CursorIsVisible

        private bool _cursorIsVisible;

        #endregion
        
        private bool _isFocusing;

        public LabelView()
        {
            // Set the text to empty string
            Text = "";

            // Set the default font.
            Font = new Font("Arial", 11);

            // When a property changes, repaint.
            Observable.Merge(
                TextChanged.Select(_ => Unit.Default),
                FontChanged.Select(_ => Unit.Default),
                ColorChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => Repaint());

            // Blink cursor if label is in edit mode.
            EditModeChanged.Select(editMode =>
            {
                if (editMode)
                {
                    return Observable.Interval(TimeSpan.FromMilliseconds(SystemInformation.CaretBlinkTime))
                        .StartWith(0);
                }
                else
                {
                    _cursorIsVisible = false;
                    // Repainting to get rid of cursor.
                    Repaint();
                    return Observable.Empty<long>();
                }
            }).Switch().Subscribe(_ =>
            {
                _cursorIsVisible = !_cursorIsVisible;
                Repaint();
            });

            // Leave edit mode if focus is lost and Repaint.
            FocusChanged.Subscribe(isFocused =>
            {
                if (!isFocused)
                {
                    IsInEditMode = false;
                }

                Repaint();
            });

            // Ignore mouse clicked when just received focus.
            FocusChanged.Where(v => v).Subscribe(_ => _isFocusing = true);

            // Update canLoseFocus when the CanLeaveEditMode is changed.
            CanLeaveEditModeChanged.Subscribe(canLoseFocus => CanLoseFocus = canLoseFocus);
            CanLeaveEditMode = true;
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            // Measure how much space it would take to fully render the
            // the string. Must be done in this function because it requires
            // a Graphics object.
            var preferredSize = g.MeasureString(Text, Font);
            PreferredWidth = (int) Math.Ceiling(preferredSize.Width);
            PreferredHeight = (int) Math.Ceiling(preferredSize.Height);

            // Draw the string.
            using (Brush brush = new SolidBrush(Color))
            {
                g.DrawString(Text, Font, brush, 0, 0);
            }
            
            using (Pen pen = new Pen(Color))
            {
                // Draw editing rectangle
                if (IsFocused)
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }

                // Draw cursor.
                if (_cursorIsVisible)
                {
                    g.DrawLine(pen, PreferredWidth - 5, 0, PreferredWidth - 5, PreferredHeight);
                }
            }
        }

        /// <summary>
        /// public accessor for focusing this label.
        /// </summary>
        public void FocusLabel()
        {
            Focus();
        }

        /// <see cref="OnKeyEvent"/>
        protected override bool OnKeyEvent(KeyEventData eventData)
        {
            if (IsInEditMode)
            {
                if (eventData.Id == KeyEvent.KEY_RELEASED &&
                    eventData.KeyCode == KeyEvent.VK_ESCAPE &&
                    CanLeaveEditMode)
                {
                    IsInEditMode = false;
                }
                else if (eventData.Id == KeyEvent.KEY_TYPED)
                {
                    // If the keyChar is backspace.
                    if (eventData.KeyChar == HexaDecimalKeyChars.BackSpace)
                    {
                        if (Text.Length > 0)
                        {
                            Text = Text.Substring(0, Text.Length - 1);
                        }
                    }
                    // If Keychar is not escape.
                    else if (char.IsLetterOrDigit(eventData.KeyChar) || eventData.KeyChar == HexaDecimalKeyChars.Colon)
                    {
                        Text += eventData.KeyChar;
                    }
                }

                return true;
            }

            return false;
        }

        /// <see cref="OnMouseEvent"/>
        protected override bool OnMouseEvent(MouseEventData eventData)
        {
            if (_isFocusing && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                _isFocusing = false;
            }
            else if (IsFocused && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                IsInEditMode = true;
                return true;
            }

            return base.OnMouseEvent(eventData);
        }
    }
}