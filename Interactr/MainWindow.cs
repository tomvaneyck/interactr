using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;
using Point = Interactr.View.Framework.Point;

namespace Interactr.View
{
    /// <summary>
    /// CanvasWindow that displays the top-level MainView.
    /// </summary>
    public class MainWindow : CanvasWindow
    {
        public MainView View { get; } = new MainView();

        public MainWindow() : base("Interactr")
        {
            this.Size = new Size(1800, 900);
            View.Width = Size.Width;
            View.Height = Size.Height;

            View.ViewModel = new MainViewModel();
            View.Focus();

            //View.RepaintRequested.Subscribe(_ => Repaint());
            View.RepaintRequested.ObserveOn(Scheduler.Default).Buffer(TimeSpan.FromMilliseconds(1000 / 120), Scheduler.Default).Subscribe(_ => Repaint());
        }

        public override void HandleKeyEvent(int id, int keyCode, char keyChar)
        {
            KeyEventData e = new KeyEventData(id, keyCode, keyChar);
            Keyboard.HandleEvent(e);
            UIElement.HandleKeyEvent(e);
        }

        public override void HandleMouseEvent(int id, int x, int y, int clickCount)
        {
            Point windowPoint = new Point(x, y);
            UIElement.HandleMouseEvent(View, new MouseEventData(id, windowPoint, clickCount));
        }

        public override void Paint(Graphics g)
        {
            View.Paint(g);
        }
    }
}
