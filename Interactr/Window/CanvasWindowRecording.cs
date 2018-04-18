using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Interactr.Window
{
    public class CanvasWindowRecording
    {
        public IList<RecordingItem> Items { get; } = new List<RecordingItem>();

        public CanvasWindowRecording() { }

        public CanvasWindowRecording(string path)
        {
            Load(path);
	    }

        public void Save(String path)
        {
            using (FileStream stream = new FileStream(path, FileMode.CreateNew))
            {
                StreamWriter writer = new StreamWriter(stream);
                Save(path, writer);
            }
	    }

        public void Save(String basePath, StreamWriter writer)
        {
		    int itemIndex = 0;
            foreach (RecordingItem item in Items)
            {
                item.Save(basePath, itemIndex++, writer);
            }
        }

        public void Load(String path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                StreamReader reader = new StreamReader(stream);
                Load(path, reader);
            }
        }

        public void Load(String basePath, StreamReader reader)
	    {
	        int i = 0;
	        string line;
	        while ((line = reader.ReadLine()) != null)
	        {
	            string[] words = line.Split(' ');
	            switch (words[0])
	            {
                    case "MouseEvent":
                        ParseMouseEvent(words);
                        break;
                    case "KeyEvent":
                        ParseKeyEvent(words);
                        break;
                    case "Paint":
                        ParsePaint(basePath, i);
                        break;
                    default:
                        throw new Exception("Invalid recording data");
	            }
	            i++;
	        }
	    }

        private void ParseMouseEvent(string[] words)
        {
            int id;
            switch (words[1])
            {
                case "MOUSE_PRESSED": id = MouseEvent.MOUSE_PRESSED; break;
                case "MOUSE_CLICKED": id = MouseEvent.MOUSE_CLICKED; break;
                case "MOUSE_RELEASED": id = MouseEvent.MOUSE_RELEASED; break;
                case "MOUSE_DRAGGED": id = MouseEvent.MOUSE_DRAGGED; break;
                default: throw new Exception("Invalid MouseEvent ID");
            }
            int x = int.Parse(words[2]);
            int y = int.Parse(words[3]);
            int clickCount = int.Parse(words[4]);
            Items.Add(new MouseEventItem(id, x, y, clickCount));
        }

        private void ParseKeyEvent(string[] words)
        {
            int id;
            switch (words[1])
            {
                case "KEY_PRESSED": id = KeyEvent.KEY_PRESSED; break;
                case "KEY_TYPED": id = KeyEvent.KEY_TYPED; break;
                default: throw new Exception("Invalid KeyEvent ID");
            }
            int keyCode = int.Parse(words[2]);
            char keyChar = (char)int.Parse(words[3]);
            Items.Add(new KeyEventItem(id, keyCode, keyChar));
        }

        private void ParsePaint(string basePath, int itemIndex)
        {
            String imagePath = PaintItem.ImagePathOf(basePath, itemIndex);
            Items.Add(new PaintItem(new Bitmap(imagePath)));
        }
	
	    public void Replay(CanvasWindow window)
        {
            int itemIndex = 0;
            foreach (RecordingItem item in Items)
            {
                item.Replay(itemIndex++, window);
            }
        }
    }
}
