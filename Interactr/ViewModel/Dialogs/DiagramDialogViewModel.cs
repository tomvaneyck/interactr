using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel.Dialogs
{
    public class DiagramDialogViewModel
    {
        #region ClassName

        private readonly ReactiveProperty<Diagram.Type> _selectedDiagramType = new ReactiveProperty<Diagram.Type>();

        public Diagram.Type SelectedDiagramType
        {
            get => _selectedDiagramType.Value;
            set => _selectedDiagramType.Value = value;
        }

        public IObservable<Diagram.Type> SelectedDiagramTypeChanged => _selectedDiagramType.Changed;

        #endregion
    }
}
