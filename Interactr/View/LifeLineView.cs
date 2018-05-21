using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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

        /// <summary>
        /// Width of each bar, in pixels
        /// </summary>
        public const int BarWidth = 12;

        /// <summary>
        /// Height of a single tick, in pixels
        /// </summary>
        public const int TickHeight = 30;

        public IReadOnlyReactiveList<ActivationBarView> ActivationBarViews { get; }

        public LifeLineView()
        {
            // Create views for activation bar viewmodels.
            ActivationBarViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.MessageStackVM.ActivationBars)
                .CreateDerivedListBinding(vm => new ActivationBarView
                {
                    ViewModel = vm,
                    TickHeight = TickHeight
                }, bar => bar.Party == ViewModel.PartyVM.Party)
                .ResultList;

            // Add activation bar views to Children.
            ActivationBarViews.OnAdd.Subscribe(e => Children.Insert(e.Index, e.Element));
            ActivationBarViews.OnDelete.Subscribe(e => Children.RemoveAt(e.Index));

            ReactiveExtensions.MergeEvents(
                ActivationBarViews.OnAdd,
                ActivationBarViews.OnDelete,
                WidthChanged
            ).Subscribe(_ => UpdateLayout());
        }

        private void UpdateLayout()
        {
            int maxY = 0;
            foreach (ActivationBarView barView in ActivationBarViews)
            {
                // Horizontally center bar on lifeline and add offset for the nesting level of the bar.
                int x = ((Width - BarWidth) / 2) + (barView.ViewModel.Level * (BarWidth / 2));
                int y = barView.ViewModel.StartTick * TickHeight;
                barView.Position = new Framework.Point(x, y);
                barView.Width = BarWidth;
                barView.Height = barView.PreferredHeight;

                maxY = Math.Max(maxY, y + barView.Height);
            }

            this.PreferredHeight = maxY;
        }

        /// <see cref="OnMouseEvent"/>
        protected override void OnMouseEventPreview(MouseEventData eventData)
        {
            var pendingMessage = ViewModel.MessageStackVM.PendingInvokingMessageVM;
            switch (eventData.Id)
            {
                case MouseEvent.MOUSE_PRESSED:
                    // User is dragging from one lifeline to another to create a new message.
                    // Create a new pending message to store this information.
                    this.Focus();
                    ViewModel.MessageStackVM.CreatePendingMessage(
                        ViewModel.PartyVM.Party, (eventData.MousePosition.Y / TickHeight) + 1);
                    eventData.IsHandled = true;
                    return;
                case MouseEvent.MOUSE_RELEASED when pendingMessage != null:
                    // User released mouse on this lifeline while dragging a new pending message.
                    // Try to create and add an actual message to the diagram.
                    pendingMessage.Receiver = ViewModel.PartyVM.Party;
                    ViewModel.MessageStackVM.FinishPendingMessage();
                    eventData.IsHandled = true;
                    return;
            }

            base.OnMouseEventPreview(eventData);
        }

        /// <see cref="PaintElement"/>
        public override void PaintElement(Graphics g)
        {
            //Draw lifeline
            int middle = Width / 2;
            g.DrawLine(Pens.Black, middle, 0, middle, Height);
        }
    }
}