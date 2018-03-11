using System;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    /// <summary>
    /// A view for a life line in a sequence diagram view.
    /// </summary>
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
            Observable.Merge(
                ViewModelChanged.ObserveNested(vm => vm.MessageStackVM.ActivationBars.OnAdd),
                ViewModelChanged.ObserveNested(vm => vm.MessageStackVM.ActivationBars.OnDelete)
            ).Subscribe(_ => Repaint());
        }

        /// <see cref="OnMouseEvent"/>
        protected override bool OnMouseEvent(MouseEventData eventData)
        {
            if (eventData.Id == MouseEvent.MOUSE_CLICKED)
            {
                ViewModel?.MessageStackVM.Diagram.Messages.Add(new Message(ViewModel.PartyVM.Party, null,
                    Message.MessageType.Invocation, "Invocation"));
                ViewModel?.MessageStackVM.Diagram.Messages.Add(new Message(ViewModel.PartyVM.Party, null,
                    Message.MessageType.Result, "Result"));
                return true;
            }

            return false;
        }

        /// <see cref="PaintElement"/>
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
                foreach (var activationBarVM in ViewModel.MessageStackVM.ActivationBars.Where(bar =>
                    bar.Party == ViewModel.PartyVM.Party))
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