using System;
using System.Reflection.Emit;
using NUnit.Framework;
using Interactr.View.Framework;
using Interactr.Window;
using Interactr.View.Controls;
using NUnit.Framework.Constraints;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class DragPanelTests
    {
        [Test]
        public void Drag1()
        {
            DragPanel dp = new DragPanel
            {
                Width = 2,
                Height = 2
            };
            RectangleView r = new RectangleView();
            r.PreferredHeight = 1;
            r.PreferredWidth = 1;
            r.CanLoseFocus = true;
            r.Position = new Point(1, 1);
            dp.Children.Add(r);
            MouseEventData mev = new MouseEventData(MouseEvent.MOUSE_PRESSED, new Point(1, 1), 1);
            UIElement.HandleMouseEvent(r, mev);
            mev = new MouseEventData(MouseEvent.MOUSE_DRAGGED, new Point(0, 0), 1);
            UIElement.HandleMouseEvent(r,mev);
            Assert.AreEqual(new Point(0,0),r.Position);
        }
        
        [Test]
        public void Drag2()
        {
            DragPanel dp = new DragPanel
            {
                Width = 10,
                Height = 10
            };
            RectangleView r = new RectangleView();
            r.PreferredHeight = 3;
            r.PreferredWidth = 3;
            r.CanLoseFocus = true;
            r.Position = new Point(3, 3);
            dp.Children.Add(r);
            MouseEventData mev = new MouseEventData(MouseEvent.MOUSE_PRESSED, new Point(3, 3), 1);
            UIElement.HandleMouseEvent(r, mev);
            mev = new MouseEventData(MouseEvent.MOUSE_DRAGGED, new Point(1, 1), 1);
            UIElement.HandleMouseEvent(r,mev);
            Assert.AreEqual(new Point(1,1),r.Position);
        }
        

    }
}
