using System;
using System.Collections.Generic;
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
        private UIElement _activeDummy;

        public ColumnsView()
        {
            // Setup panels
            this.Children.Add(ColumnsPanel);

            var reorderingPanel = new DragPanel
            {
                IsVisibleToMouse = false
            };
            this.Children.Add(reorderingPanel);

            // When a child receives MOUSE_PRESSED, start a drag.
            ColumnsPanel.Children.ObserveEach(c => c.MouseEventOccured)
                .Where(e => e.Value.Id == MouseEvent.MOUSE_PRESSED)
                .Subscribe(e =>
                {
                    // Replace the element from the stack with a dummy and the element to the dragpanel
                    int indexOfSelectedChild = ColumnsPanel.Children.IndexOf(e.Element);
                    _activeDummy = new UIElement
                    {
                        PreferredWidth = e.Element.PreferredWidth,
                        PreferredHeight = e.Element.PreferredHeight
                    };
                    ColumnsPanel.Children[indexOfSelectedChild] = _activeDummy;
                    reorderingPanel.Children.Add(e.Element);

                    // Make sure dragpanel receives mouse events
                    reorderingPanel.IsVisibleToMouse = true;
                }
            );
            
            // When the dragging finishes, move the element in the stack
            reorderingPanel.OnDragFinished.Subscribe(dragElement =>
            {
                // Reset the drag panel
                reorderingPanel.Children.Remove(dragElement);
                reorderingPanel.IsVisibleToMouse = false;

                // Find the position to insert the element at.
                int curX = 0;
                for (var i = 0; i < ColumnsPanel.Children.Count; i++)
                {
                    var child = ColumnsPanel.Children[i];
                    curX += child.PreferredWidth;
                    if (curX >= dragElement.Position.X)
                    {
                        // Insert dragged element at i.
                        ColumnsPanel.Children.Insert(i, dragElement);
                    }
                }

                // Remove dummy element.
                ColumnsPanel.Children.Remove(_activeDummy);
            });
        }
    }
}
