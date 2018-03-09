using System;
using System.Drawing;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    public class LifeLineView : UIElement
    {
        #region ViewModel
        private readonly ReactiveProperty<LifeLineViewModel> _viewModel = new ReactiveProperty<LifeLineViewModel>();
        public LifeLineViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }
        public IObservable<LifeLineViewModel> ViewModelChanged => _viewModel.Changed;
        #endregion

        public LifeLineView()
        {

        }

        protected override bool OnMouseEvent(MouseEventData eventData)
        {
            if (eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                ViewModel?.MessageStackVM.Diagram.Messages.Add(new Message(ViewModel.PartyVM.Party, null, Message.MessageType.Invocation, "Invocation"));
                ViewModel?.MessageStackVM.Diagram.Messages.Add(new Message(ViewModel.PartyVM.Party, null, Message.MessageType.Result, "Result"));
                return true;
            }

            return false;
        }

        public override void PaintElement(Graphics g)
        {
            //Draw lines
            int middle = Width / 2;
            g.DrawLine(Pens.Black, middle, 0, middle, Height);

            //Draw activation bars
            if (ViewModel?.MessageStackVM != null)
            {
                int barWidth = 12;
                int tickHeight = 30;
                foreach (var activationBarVM in ViewModel.MessageStackVM.ActivationBars)
                {
                    int x = middle - (barWidth / 2);
                    int y = (activationBarVM.StartTick) * tickHeight;
                    int height = (activationBarVM.EndTick - activationBarVM.StartTick) * tickHeight;
                    g.FillRectangle(Brushes.White, x, y, barWidth, height);
                    g.DrawRectangle(Pens.Black, x, y, barWidth, height);
                }
            }
        }
    }
}
