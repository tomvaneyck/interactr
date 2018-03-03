using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Window;

namespace Interactr.View
{
    public class MainView : CanvasWindow
    {
        public MainView() : base("Interactr")
        {
            
        }

        public override void HandleKeyEvent(int id, int keyCode, char keyChar)
        {
            base.HandleKeyEvent(id, keyCode, keyChar);
        }

        public override void HandleMouseEvent(int id, int x, int y, int clickCount)
        {
            base.HandleMouseEvent(id, x, y, clickCount);
        }

        public override void Paint(Graphics g)
        {
            base.Paint(g);
        }
    }
}
