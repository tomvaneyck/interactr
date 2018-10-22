using System.Collections.Generic;
using Interactr.Window;

namespace Interactr.View.Framework
{
    /// <summary>
    /// Keeps track of keyboard state.
    /// </summary>
    public class Keyboard
    {
        private static readonly Dictionary<int, bool> KeyStates = new Dictionary<int, bool>();

        /// <summary>
        /// Update the stored keyboard state with an event.
        /// </summary>
        /// <param name="e">The keyboard event</param>
        public static void HandleEvent(KeyEventData e)
        {
            switch (e.Id)
            {
                case KeyEvent.KEY_PRESSED:
                    KeyStates[e.KeyCode] = true;
                    break;
                case KeyEvent.KEY_RELEASED:
                    KeyStates[e.KeyCode] = false;
                    break;
            }
        }

        /// <summary>
        /// Return true if the specified key is pressed.
        /// </summary>
        /// <param name="keyCode">The keycode of the key, as found in KeyEventData</param>
        /// <returns>True if the key is pressed, else false.</returns>
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
