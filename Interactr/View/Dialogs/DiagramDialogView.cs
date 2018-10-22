using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.ViewModel.Dialogs;

namespace Interactr.View.Dialogs
{
    public class DiagramDialogView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<DiagramDialogViewModel> _viewModel = new ReactiveProperty<DiagramDialogViewModel>();

        public DiagramDialogViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<DiagramDialogViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public DiagramDialogView()
        {
            IsTabScope = true;

            // Setup radio button controls
            RadioButtonGroup.RadioButton sequenceDiagramButton = new RadioButtonGroup.RadioButton
            {
                Label = "Sequence diagram"
            };
            RadioButtonGroup.RadioButton communicationDiagramButton = new RadioButtonGroup.RadioButton
            {
                Label = "Communication diagram"
            };

            RadioButtonGroup diagramTypeRadioGroup = new RadioButtonGroup
            {
                Children =
                {
                    sequenceDiagramButton,
                    communicationDiagramButton
                }
            };
            this.Children.Add(diagramTypeRadioGroup);

            // Setup radio button two-way bindings
            sequenceDiagramButton.IsSelectedChanged.Where(isSelected => isSelected).Subscribe(_ =>
            {
                ViewModel.SelectedDiagramType = Diagram.Type.Sequence;
            });

            communicationDiagramButton.IsSelectedChanged.Where(isSelected => isSelected).Subscribe(_ =>
            {
                ViewModel.SelectedDiagramType = Diagram.Type.Communication;
            });

            ViewModelChanged.ObserveNested(vm => vm.SelectedDiagramTypeChanged).Subscribe(selectedType =>
            {
                switch (selectedType)
                {
                    case Diagram.Type.Sequence:
                        sequenceDiagramButton.IsSelected = true;
                        break;
                    case Diagram.Type.Communication:
                        communicationDiagramButton.IsSelected = true;
                        break;
                }
            });
        }
    }
}
