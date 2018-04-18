using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Interactr.Window
{
    class PaintItem : RecordingItem
    {
        public Bitmap Image { get; }

        public PaintItem(Bitmap image)
        {
            Image = image;
        }

        public static string ImagePathOf(string basePath, int itemIndex)
        {
            return basePath + ".image" + itemIndex + ".png";
        }

        public override void Save(string path, int itemIndex, StreamWriter writer)
        {
            string imagePath = ImagePathOf(path, itemIndex);
            Image.Save(imagePath, ImageFormat.Png);
            writer.WriteLine("Paint");
        }

        public override void Replay(int itemIndex, CanvasWindow window)
        {
            Bitmap img = window.CaptureImage();
            if (!CheckImageEqual(img))
            {
                throw new Exception($"Replay: Paint item {itemIndex} does not match.");
            }
        }

        private bool CheckImageEqual(Bitmap img)
        {
            if (img.Size != Image.Size)
            {
                return false;
            }

            //Inefficient method:
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color c1 = img.GetPixel(x, y);
                    Color c2 = Image.GetPixel(x, y);
                    if (c1 != c2)
                    {
                        return false;
                    }
                }
            }
            return true;

            //Efficient method:
            /*var bm1 = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bm2 = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                //Compare bm1 and bm2
            }
            finally
            {
                Image.UnlockBits(bm1);
                img.UnlockBits(bm2);
            }*/
        }
    }
}
