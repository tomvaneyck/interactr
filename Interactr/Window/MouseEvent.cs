using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Window
{
    public static class MouseEvent
    {
        public const int MOUSE_FIRST = 500;
        public const int MOUSE_LAST = 507;
        public const int MOUSE_CLICKED = MOUSE_FIRST;
        public const int MOUSE_PRESSED = 1 + MOUSE_FIRST; //Event.MOUSE_DOWN
        public const int MOUSE_RELEASED = 2 + MOUSE_FIRST; //Event.MOUSE_UP
        public const int MOUSE_MOVED = 3 + MOUSE_FIRST; //Event.MOUSE_MOVE
        public const int MOUSE_ENTERED = 4 + MOUSE_FIRST; //Event.MOUSE_ENTER
        public const int MOUSE_EXITED = 5 + MOUSE_FIRST; //Event.MOUSE_EXIT
        public const int MOUSE_DRAGGED = 6 + MOUSE_FIRST; //Event.MOUSE_DRAG
        public const int MOUSE_WHEEL = 7 + MOUSE_FIRST;
        public const int NOBUTTON = 0;
        public const int BUTTON1 = 1;
        public const int BUTTON2 = 2;
        public const int BUTTON3 = 3;
    }
}
