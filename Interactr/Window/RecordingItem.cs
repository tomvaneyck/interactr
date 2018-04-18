using System;
using System.IO;

namespace Interactr.Window
{
    public abstract class RecordingItem
    {
        public abstract void Save(String path, int itemIndex, StreamWriter writer);
        public abstract void Replay(int itemIndex, CanvasWindow window);
    }
}
