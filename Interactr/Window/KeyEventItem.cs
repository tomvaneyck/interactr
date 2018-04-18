using System.IO;

namespace Interactr.Window
{
    public class KeyEventItem : RecordingItem
    {
        public int Id { get; }
        public int KeyCode { get; }
        public char KeyChar { get; }

        public KeyEventItem(int id, int keyCode, char keyChar)
        {
            Id = id;
            KeyCode = keyCode;
            KeyChar = keyChar;
        }

        public override void Save(string path, int itemIndex, StreamWriter writer)
        {
            string id;
            switch (Id)
            {
                case KeyEvent.KEY_PRESSED: id = "KEY_PRESSED"; break;
                case KeyEvent.KEY_TYPED: id = "KEY_TYPED"; break;
                default: id = "unknown"; break;
            }
            writer.WriteLine($"KeyEvent {id} {KeyCode} {(int)KeyChar}");
        }

        public override void Replay(int itemIndex, CanvasWindow window)
        {
            window.HandleKeyEvent(Id, KeyCode, KeyChar);
        }
    }
}
