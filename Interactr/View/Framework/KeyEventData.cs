namespace Interactr.View.Framework
{
    /// <summary>
    /// Information about a keyboard input event.
    /// </summary>
    public class KeyEventData
    {
        /// <summary>
        /// The type of key event.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The key identifier.
        /// </summary>
        public int KeyCode { get; }

        /// <summary>
        /// The character associated with this key.
        /// </summary>
        public char KeyChar { get; }

        public KeyEventData(int id, int keyCode, char keyChar)
        {
            Id = id;
            KeyCode = keyCode;
            KeyChar = keyChar;
        }
    }
}