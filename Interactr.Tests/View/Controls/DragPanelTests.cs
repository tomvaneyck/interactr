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
                Width = 20,
                Height = 20
            };
            RectangleView r = new RectangleView
            {
                PreferredHeight = 10,
                PreferredWidth = 10,
                Position = new Point(10, 10)
            };
            dp.Children.Add(r);

            MouseEventData mev = new MouseEventData(MouseEvent.MOUSE_PRESSED, new Point(10, 10), 1);
            UIElement.HandleMouseEvent(dp, mev);

            mev = new MouseEventData(MouseEvent.MOUSE_DRAGGED, new Point(0, 0), 1);
            UIElement.HandleMouseEvent(dp, mev);
            Assert.AreEqual(new Point(0, 0), r.Position);

            mev = new MouseEventData(MouseEvent.MOUSE_RELEASED, new Point(0, 0), 1);
            UIElement.HandleMouseEvent(dp, mev);
            Assert.AreEqual(new Point(0, 0), r.Position);
        }

        [Test]
        public void Drag2()
        {
            DragPanel dp = new DragPanel
            {
                Width = 10,
                Height = 10
            };
            RectangleView r = new RectangleView
            {
                PreferredHeight = 3,
                PreferredWidth = 3,
                Position = new Point(3, 3)
            };
            dp.Children.Add(r);

            MouseEventData mev = new MouseEventData(MouseEvent.MOUSE_PRESSED, new Point(3, 3), 1);
            UIElement.HandleMouseEvent(dp, mev);
            
            mev = new MouseEventData(MouseEvent.MOUSE_DRAGGED, new Point(1, 1), 1);
            UIElement.HandleMouseEvent(dp, mev);
            Assert.AreEqual(new Point(1, 1), r.Position);

            mev = new MouseEventData(MouseEvent.MOUSE_RELEASED, new Point(1, 1), 1);
            UIElement.HandleMouseEvent(dp, mev);
            Assert.AreEqual(new Point(1, 1), r.Position);
        }
    }
}
