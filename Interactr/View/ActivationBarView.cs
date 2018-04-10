using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    public class ActivationBarView : RectangleView
    {
        #region ViewModel

        private readonly ReactiveProperty<ActivationBarViewModel> _viewModel = new ReactiveProperty<ActivationBarViewModel>();

        public ActivationBarViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<ActivationBarViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        #region TickHeight

        private readonly ReactiveProperty<int> _tickHeight = new ReactiveProperty<int>();

        /// <summary>
        /// Height of each tick, in pixels
        /// </summary>
        public int TickHeight
        {
            get => _tickHeight.Value;
            set => _tickHeight.Value = value;
        }

        public IObservable<int> TickHeightChanged => _tickHeight.Changed;

        #endregion

        public ActivationBarView()
        {
            CanBeFocused = false;
            BackgroundColor = Color.White;

            // How long is this activation bar, in ticks?
            var tickCount = Observable.CombineLatest(
                ViewModelChanged.Where(vm => vm != null).Select(vm => vm.StartTick),
                ViewModelChanged.Where(vm => vm != null).Select(vm => vm.EndTick),
                (startTick, endTick) => endTick - startTick
            );

            // How long is this activation bar, in pixels?
            var totalHeight = Observable.CombineLatest(
                tickCount,
                TickHeightChanged,
                (ticks, height) => ticks * height
            );

            // Set the preferred height.
            totalHeight.Subscribe(height => PreferredHeight = height);
        }
    }
}
