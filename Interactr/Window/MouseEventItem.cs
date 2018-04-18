using System.IO;

namespace Interactr.Window
{
    public class MouseEventItem : RecordingItem
    {
        public int Id { get; }
        public int X { get; }
        public int Y { get; }
        public int ClickCount { get; }

        public MouseEventItem(int id, int x, int y, int clickCount)
        {
            Id = id;
            X = x;
            Y = y;
            ClickCount = clickCount;
        }

        public override void Save(string path, int itemIndex, StreamWriter writer)
        {
            string id;
            switch (Id)
            {
                case MouseEvent.MOUSE_CLICKED: id = "MOUSE_CLICKED"; break;
                case MouseEvent.MOUSE_PRESSED: id = "MOUSE_PRESSED"; break;
                case MouseEvent.MOUSE_RELEASED: id = "MOUSE_RELEASED"; break;
                case MouseEvent.MOUSE_DRAGGED: id = "MOUSE_DRAGGED"; break;
                default: id = "unknown"; break;
            }

            writer.WriteLine($"MouseEvent {id} {X} {Y} {ClickCount}");
        }

        public override void Replay(int itemIndex, CanvasWindow window)
        {
            window.HandleMouseEvent(Id, X, Y, ClickCount);
        }
    }
}
