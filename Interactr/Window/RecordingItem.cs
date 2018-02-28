using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Window
{
    public abstract class RecordingItem
    {
        public abstract void Save(String path, int itemIndex, StreamWriter writer);
        public abstract void Replay(int itemIndex, CanvasWindow window);
    }
}
