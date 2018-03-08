using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A view for displaying and editing text.
    /// </summary>
    public class LabelView : UIElement
    {
        #region Text
        /// <summary>
        /// The text that is displayed.
        /// </summary>
        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }
        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();
        public IObservable<string> TextChanged => _text.Changed;
        #endregion

        #region Font
        /// <summary>
        /// The font used to render the label.
        /// </summary>
        public Font Font
        {
            get => _font.Value;
            set => _font.Value = value;
        }
        private readonly ReactiveProperty<Font> _font = new ReactiveProperty<Font>();
        public IObservable<Font> FontChanged => _font.Changed;
        #endregion

        #region IsInEditMode
        public bool IsInEditMode
        {
            get => _isInEditMode.Value;
            set => _isInEditMode.Value = value;
        }
        private readonly ReactiveProperty<bool> _isInEditMode = new ReactiveProperty<bool>();
        public IObservable<bool> EditModeChanged => _isInEditMode.Changed;
        #endregion

        #region CanLeaveEditMode
        public bool CanLeaveEditMode
        {
            get => _canLeaveEditMode.Value;
            set => _canLeaveEditMode.Value = value;
        }
        private readonly ReactiveProperty<bool> _canLeaveEditMode = new ReactiveProperty<bool>();
        public IObservable<bool> CanLeaveEditModeChanged => _canLeaveEditMode.Changed;
        #endregion

        private bool _cursorIsVisible = false;

        public LabelView()
        {
            // Set the default font.
            Font = new Font("Arial", 11);

            // When a property changes, repaint.
            Observable.Merge(
                TextChanged.Select(_ => Unit.Default),
                FontChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => Repaint());

            // Blink cursor if label is in edit mode.
            EditModeChanged.Select(editMode =>
            {
                if (editMode)
                {
                    return Observable.Interval(TimeSpan.FromMilliseconds(SystemInformation.CaretBlinkTime)).StartWith(0);
                }
                else
                {
                    _cursorIsVisible = false;
                    return Observable.Empty<long>();
                }
            }).Switch().Subscribe(_ =>
            {
                _cursorIsVisible = !_cursorIsVisible;
                Repaint();
            });

            // Block focus change if not allowed to leave edit mode and repaint.
            FocusChanged.Subscribe(isFocused =>
            {
                if (!isFocused)
                {
                    IsInEditMode = false;
                }
                Repaint();
            });

            // Block focus losing when canLeaveEditMode false.
            CanLeaveEditModeChanged.Subscribe(canLoseFocus => CanLoseFocus = canLoseFocus);

            CanLeaveEditMode = true;
        }

        public override void PaintElement(Graphics g)
        {
            // Measure how much space it would take to fully render the
            // the string. Must be done in this function because it requires
            // a Graphics object.
            var preferredSize = g.MeasureString(Text, Font);
            PreferredWidth = (int)Math.Ceiling(preferredSize.Width);
            PreferredHeight = (int)Math.Ceiling(preferredSize.Height);

            if (IsFocused)
            {
                g.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);
            }

            // Draw the string.
            g.DrawString(Text, Font, Brushes.Black, 0, 0);

            // Draw cursor.
            if (_cursorIsVisible)
            {
                g.DrawLine(Pens.Black, PreferredWidth - 5, 0, PreferredWidth - 5, PreferredHeight);
            }
        }

        protected override bool OnMouseEvent(MouseEventData eventData)
        {
            if (IsFocused && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                IsInEditMode = true;
                return true;
            }
            return base.OnMouseEvent(eventData);
        }
    }
}
