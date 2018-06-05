using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.Window;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Interactr.View.Framework.Point;

namespace Interactr.Tests
{
    [TestFixture]
    class ScenarioTestCase
    {
        [Test]
        public void Test()
        {
            var windowsView = new WindowsView();
            var button = new Button()
            {
                Label = "Add"
            };
            var textBoxTerm1 = new TextBox();
            var textBoxTerm2 = new TextBox();
            var solutionLabel = new LabelView();

            var panel = new StackPanel()
            {
                Children = { textBoxTerm1, textBoxTerm2, button, solutionLabel },
                StackOrientation = Orientation.Vertical
            };

            windowsView.AddWindow(panel);

            button.OnButtonClick.Subscribe(_ =>
            {
                if (!int.TryParse(textBoxTerm1.Text, out int term1) ||
                    !int.TryParse(textBoxTerm2.Text, out int term2))
                {
                    solutionLabel.Text = "Invalid input";
                }
                else
                {
                    solutionLabel.Text = (term1 + term2).ToString();
                }
            });

            textBoxTerm1.Text = 5.ToString();
            textBoxTerm2.Text = 6.ToString();
            UIElement.HandleMouseEvent(windowsView,
                new MouseEventData(MouseEvent.MOUSE_RELEASED, button.TranslatePointTo(windowsView, button.Position), 1));
            Assert.AreEqual("11", solutionLabel.Text);

            textBoxTerm1.Text = "term 1";
            textBoxTerm2.Text = "term 2";
            UIElement.HandleMouseEvent(windowsView,
                new MouseEventData(MouseEvent.MOUSE_RELEASED, button.TranslatePointTo(windowsView, button.Position), 1));
            Assert.AreEqual("Invalid input", solutionLabel.Text);

            //var main = new MainWindow(windowsView);
        }

        #region Visual CanvasWindow

        [STAThread, Test]
        public static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            new ScenarioTestCase().Test();
        }

        /// <summary>
        /// CanvasWindow that displays the top-level MainView.
        /// </summary>
        public class MainWindow : CanvasWindow
        {
            private UIElement _view;

            public MainWindow(UIElement view) : base("Interactr")
            {
                _view = view;

                this.Size = new Size(1800, 900);
                view.Width = Size.Width;
                view.Height = Size.Height;

                view.Focus();

                view.RepaintRequested.ObserveOn(Scheduler.Default).Buffer(TimeSpan.FromMilliseconds(1000 / 120), Scheduler.Default).Subscribe(_ => Repaint());
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
                UIElement.HandleMouseEvent(_view, new MouseEventData(id, windowPoint, clickCount));
            }

            public override void Paint(Graphics g)
            {
                _view.Paint(g);
            }
        }

        #endregion
    }
}
