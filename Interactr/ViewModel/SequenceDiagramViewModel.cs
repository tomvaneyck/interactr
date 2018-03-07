using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class SequenceDiagramViewModel
    {
        #region IsVisible
        private readonly ReactiveProperty<bool> _isVisible = new ReactiveProperty<bool>();
        public bool IsVisible
        {
            get => _isVisible.Value;
            set => _isVisible.Value = value;
        }
        public IObservable<bool> IsVisibleChanged => _isVisible.Changed;
        #endregion
    }
}
