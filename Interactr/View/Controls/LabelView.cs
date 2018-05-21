using System;
using System.Diagnostics;
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
    public class LabelView : EditableText
    {
        #region IsReadOnly

        private readonly ReactiveProperty<bool> _isReadOnly = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if the label is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly.Value;
            set => _isReadOnly.Value = value;
        }

        /// <summary>
        /// Emit the new IsReadOnly values.
        /// </summary>
        public IObservable<bool> IsReadOnlyChanged => _isReadOnly.Changed;

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

        private bool _isFocusing;

        public LabelView() : base()
        {
            // Leave edit mode if ReadOnly is activated
            IsReadOnlyChanged.Where(isReadOnly => isReadOnly == true).Subscribe(i => { IsInEditMode = false; });

            // Ignore mouse clicked when just received focus.
            FocusChanged.Where(v => v).Subscribe(_ => _isFocusing = true);

            // Update canLoseFocus when the CanLeaveEditMode is changed.
            CanLeaveEditModeChanged.Subscribe(canLoseFocus => CanLoseFocus = canLoseFocus);
            CanLeaveEditMode = true;
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            base.PaintElement(g);

            // Draw focus rectangle
            if (IsFocused)
            {
                using (Pen pen = new Pen(Color))
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            }
        }

        protected override void OnKeyEvent(KeyEventData eventData)
        {
            if (IsInEditMode &&
                    eventData.Id == KeyEvent.KEY_RELEASED &&
                    eventData.KeyCode == KeyEvent.VK_ESCAPE &&
                    CanLeaveEditMode)
            {
                IsInEditMode = false;
                eventData.IsHandled = true;
            }
            else
            {
                base.OnKeyEvent(eventData);
            }
        }

        /// <see cref="EditableText.HandleMouseEvent"/>
        protected override void HandleMouseEvent(MouseEventData eventData)
        {
            if (_isFocusing && eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                _isFocusing = false;
                eventData.IsHandled = true;
            }
            else if (IsFocused && eventData.Id == MouseEvent.MOUSE_CLICKED )
            {
                IsInEditMode = true;
                eventData.IsHandled = true;
            }
        }
    }
}