using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    /// A UIElement with a list of RadioButtons.
    /// Only one RadioButton in this list can be active at a time.
    /// </summary>
    public class RadioButtonGroup : StackPanel
    {
        public RadioButtonGroup()
        {
            // Default values.
            StackOrientation = Orientation.Vertical;

            // Only one radiobutton can be selected at a time.
            var onRadioButtonSelectedChanged = this.Children.ObserveWhere(
                elem => ((RadioButton) elem).IsSelectedChanged, 
                elem => elem is RadioButton
            );

            onRadioButtonSelectedChanged
                .Where(e => e.Value) // When a button is selected
                .Subscribe(e =>
                {
                    //Unselect all others
                    foreach (var radioButton in Children.OfType<RadioButton>().Where(r => r != e.Element))
                    {
                        radioButton.IsSelected = false;
                    }
                });
        }

        /// <summary>
        /// A button-like UIElement that can be selected, but not unselected, by clicking it.
        /// </summary>
        public class RadioButton : UIElement
        {
            private const int ButtonSize = 12; // Size of the button in pixels.
            private const int SelectionDotSize = 4; // Size of the selection dot in pixels.
            private const int LabelMargin = 6; // Space between the button and the label.

            #region IsSelected
            private readonly ReactiveProperty<bool> _isSelected = new ReactiveProperty<bool>();

            /// <summary>
            /// True if this radiobutton is selected, otherwise false.
            /// </summary>
            public bool IsSelected
            {
                get => _isSelected.Value;
                set => _isSelected.Value = value;
            }

            public IObservable<bool> IsSelectedChanged => _isSelected.Changed;
            #endregion
            
            #region Label
            private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

            /// <summary>
            /// The text to display next to the radio button
            /// </summary>
            public string Label
            {
                get => _label.Value;
                set => _label.Value = value;
            }

            public IObservable<string> LabelChanged => _label.Changed;
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

            public RadioButton()
            {
                // Default values.
                Font = new Font("Arial", 11);

                // Repaint when a property changes.
                ReactiveExtensions.MergeEvents(
                    IsSelectedChanged,
                    LabelChanged,
                    FontChanged
                ).Subscribe(_ => Repaint());

                // Set the preferred width and height by measuring how much space it
                // would take to fully render the string + extra space for the button.
                ReactiveExtensions.MergeEvents(LabelChanged, FontChanged).Subscribe(_ =>
                {
                    PreferredWidth = TextRenderer.MeasureText(Label, Font).Width + ButtonSize + LabelMargin;
                    PreferredHeight = Math.Max(TextRenderer.MeasureText(Label, Font).Height, ButtonSize);
                });
            }

            protected override void OnMouseEvent(MouseEventData eventData)
            {
                // On click, mark this button as selected.
                if (eventData.Id == MouseEvent.MOUSE_PRESSED)
                {
                    Focus();
                    IsSelected = true;
                    eventData.IsHandled = true;
                    return;
                }
                base.OnMouseEvent(eventData);
            }

            protected override void OnKeyEvent(KeyEventData eventData)
            {
                if (eventData.Id == KeyEvent.KEY_PRESSED && eventData.KeyCode == KeyEvent.VK_SPACE)
                {
                    IsSelected = true;
                    eventData.IsHandled = true;
                    return;
                }
                base.OnKeyEvent(eventData);
            }

            public override void PaintElement(Graphics g)
            {
                int y = (this.Height - ButtonSize) / 2;
                g.FillEllipse(Brushes.White, 0, y, ButtonSize, ButtonSize);
                g.DrawEllipse(Pens.Black, 0, y, ButtonSize, ButtonSize);

                if (IsSelected)
                {
                    g.FillEllipse(
                        Brushes.Black, 
                        (ButtonSize - SelectionDotSize) / 2, 
                        y + ((ButtonSize - SelectionDotSize) / 2),
                        SelectionDotSize,
                        SelectionDotSize
                    );
                }

                g.DrawString(Label, Font, Brushes.Black, ButtonSize + LabelMargin, 0);
            }
        }
    }
}
