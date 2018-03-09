using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.ViewModel;

namespace Interactr.View.Controls
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

        public override void PaintElement(Graphics g)
        {
            //Draw lines
            g.DrawLine(Pens.Black, Width/2, 0, Width/2, Height);

            //Draw activation bars
            //TODO
        }
    }
}
