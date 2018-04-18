using System;
using NUnit.Framework;
using Interactr.View.Framework;
using Interactr.Window;
using Interactr.View.Controls;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class DragPanelTests
    {
        [Test]
        public void Test()
        {
            DragPanel dp = new DragPanel();
            dp.Position = new Point(1, 1);
            MouseEventData mev = new MouseEventData(MouseEvent.MOUSE_DRAGGED, new Point(0, 0), 1);
            UIElement.HandleMouseEvent(dp,mev);
            Assert.AreEqual(new Point(0,0),dp.Position);
        }

    }
}
