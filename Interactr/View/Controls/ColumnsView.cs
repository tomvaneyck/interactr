using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.Window;

namespace Interactr.View.Controls
{
    /// <summary>
    /// Has horizontally stacked children like a StackPanel, but these can be reordered by the user by dragging.
    /// </summary>
    public class ColumnsView : AnchorPanel
    {
        public StackPanel ColumnsPanel { get; } = new StackPanel();
        private DragPanel _reorderingPanel;
        private UIElement _activeDummy;

        public ColumnsView()
        {
            // Setup panels
            this.Children.Add(ColumnsPanel);

            _reorderingPanel = new DragPanel
            {
                IsVisibleToMouse = false,
                DraggableOrientations = Orientation.Horizontal
            };
            this.Children.Add(_reorderingPanel);

            // When a child receives MOUSE_PRESSED, start a drag.
            ColumnsPanel.Children.ObserveEach(c => c.MouseEventOccured)
                .Where(e => e.Value.Id == MouseEvent.MOUSE_PRESSED)
                .Subscribe(e =>
                {
                    StartDrag(e.Element, e.Element.TranslatePointTo(_reorderingPanel, e.Value.MousePosition));

                    // Mark event as handled
                    e.Value.IsHandled = true;
                }
            );
            
            // When the dragging finishes, move the element in the stack
            _reorderingPanel.OnDragFinished.Subscribe(EndDrag);
        }

        private void StartDrag(UIElement element, Point mousePosition)
        {
            // Replace the element from the stack with a dummy and add the element to the dragpanel
            int indexOfSelectedChild = ColumnsPanel.Children.IndexOf(element);
            _activeDummy = new UIElement
            {
                PreferredWidth = element.PreferredWidth,
                PreferredHeight = element.PreferredHeight
            };
            ColumnsPanel.Children[indexOfSelectedChild] = _activeDummy;
            _reorderingPanel.Children.Add(element);

            // Start the drag of the element.
            _reorderingPanel.StartDrag(element, mousePosition);

            // Make sure dragpanel receives mouse events.
            _reorderingPanel.IsVisibleToMouse = true;
        }

        private void EndDrag(UIElement dragElement)
        {
            _reorderingPanel.Children.Remove(dragElement);
            _reorderingPanel.IsVisibleToMouse = false;

            // Find the position to insert the element at.
            int insertionIndex = ColumnsPanel.Children.Count; // Append by default
            int curX = 0;
            for (var i = 0; i < ColumnsPanel.Children.Count; i++)
            {
                var child = ColumnsPanel.Children[i];
                if (curX + (child.PreferredWidth / 2) >= (dragElement.Position.X + (dragElement.PreferredWidth / 2)))
                {
                    // Insert dragged element at i.
                    insertionIndex = i;
                    break;
                }
                curX += child.PreferredWidth;
            }
            ColumnsPanel.Children.Insert(insertionIndex, dragElement);

            // Remove dummy element.
            ColumnsPanel.Children.Remove(_activeDummy);
            _activeDummy = null;
        }
    }
}
