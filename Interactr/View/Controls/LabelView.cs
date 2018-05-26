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

        public LabelView() : base()
        {
            // Leave edit mode if ReadOnly is activated
            IsReadOnlyChanged.Where(isReadOnly => isReadOnly == true).Subscribe(i => { IsInEditMode = false; });

            // In edit mode, capture the mouse on the diagram editor scope.
            EditModeChanged.Subscribe(editMode =>
            {
                UIElement mouseCaptureScope =
                    WalkToRoot().OfType<DiagramEditorView>().FirstOrDefault() ?? WalkToRoot().Last();

                mouseCaptureScope.MouseCapturingElement = editMode ? this : null;
            });
        }

        /// <param name="isFocused"></param>
        /// <see cref="HandleFocusChange">
        protected override void HandleFocusChange(bool isFocused)
        {
            if (!isFocused && CanLeaveEditMode)
            {
                IsInEditMode = false;
            }
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            base.PaintElement(g);

            // Draw editing or focus rectangle.
            if (IsInEditMode)
            {
                using (Pen pen = new Pen(Color.DodgerBlue))
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            }
            else if (IsFocused)
            {
                using (Pen pen = new Pen(Color))
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            }
        }

        /// <see cref="OnKeyEvent"/>
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

        protected override void OnMouseEvent(MouseEventData e)
        {
            // When the user clicks outside of the label bounds, try to exit edit mode.
            if (IsInEditMode)
            {
                bool eventOutOfLabelBounds = e.MousePosition.X < 0 || e.MousePosition.Y < 0 ||
                                             e.MousePosition.X >= Width || e.MousePosition.Y >= Height;

                if (e.Id == MouseEvent.MOUSE_PRESSED && eventOutOfLabelBounds && CanLeaveEditMode)
                {
                    IsInEditMode = false;
                }

                e.IsHandled = true;
            }
            
            // When the label is focused and the user clicks the label, enter edit mode.
            else if (IsFocused && e.Id == MouseEvent.MOUSE_PRESSED && !IsReadOnly)
            {
                IsInEditMode = true;
                e.IsHandled = true;
            }

            if (e.Id == MouseEvent.MOUSE_PRESSED && CanBeFocused)
            {
                Focus();
                e.IsHandled = true;
            }
        }
    }
}