using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Window;

namespace Interactr.View.Framework
{
    public class Keyboard
    {
        private static readonly Dictionary<int, bool> KeyStates = new Dictionary<int, bool>();

        public static void HandleEvent(int id, int keyCode)
        {
            switch (id)
            {
                case KeyEvent.KEY_PRESSED:
                    KeyStates[keyCode] = true;
                    break;
                case KeyEvent.KEY_RELEASED:
                    KeyStates[keyCode] = false;
                    break;
            }
        }

        public static bool IsKeyDown(int keyCode)
        {
            if (KeyStates.TryGetValue(keyCode, out bool state))
            {
                return state;
            }

            return false;
        }
    }
}
