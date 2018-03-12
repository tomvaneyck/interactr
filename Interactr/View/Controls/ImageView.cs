using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Framework;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A view that displays an image.
    /// </summary>
    public class ImageView : UIElement
    {
        #region Image

        private readonly ReactiveProperty<Image> _image = new ReactiveProperty<Image>();

        /// <summary>
        /// The image to display.
        /// </summary>
        public Image Image
        {
            get => _image.Value;
            set => _image.Value = value;
        }

        public IObservable<Image> ImageChanged => _image.Changed;

        #endregion

        public ImageView()
        {
            //When the image changes, change to preferred size and repaint.
            ImageChanged.Subscribe(newImage =>
            {
                if (newImage != null)
                {
                    PreferredWidth = newImage.Width;
                    PreferredHeight = newImage.Height;
                }

                Repaint();
            });
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            if (Image == null)
            {
                // Draw a pink rectangle if no image was set.
                g.FillRectangle(Brushes.Pink, 0, 0, this.Width, this.Height);
                return;
            }

            g.DrawImage(Image, 0, 0, this.Width, this.Height);
        }
    }
}