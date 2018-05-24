using System;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.ViewModel.Dialogs;

namespace Interactr.ViewModel
{
    public class DiagramEditorViewModel
    {
        /// <summary>
        /// The underlying diagram associated with the diagram view model.
        /// </summary>
        /// <remarks>The underlying diagram can not be changed after the diagramViewModel construction.</remarks>
        public Diagram Diagram { get; }

        /// <summary>
        /// The communication diagram view model.
        /// </summary>
        public CommunicationDiagramViewModel CommDiagramVM { get; }

        /// <summary>
        /// The sequence diagram view model.
        /// </summary>
        public SequenceDiagramViewModel SeqDiagramVM { get; }
        
        #region VisibleDiagramType

        private readonly ReactiveProperty<Diagram.Type> _visibleDiagramType = new ReactiveProperty<Diagram.Type>();

        public Diagram.Type VisibleDiagramType
        {
            get => _visibleDiagramType.Value;
            set => _visibleDiagramType.Value = value;
        }

        public IObservable<Diagram.Type> VisibleDiagramTypeChanged => _visibleDiagramType.Changed;

        #endregion

        public DiagramEditorViewModel(Diagram diagram)
        {
            Diagram = diagram;
            CommDiagramVM = new CommunicationDiagramViewModel(diagram);
            SeqDiagramVM = new SequenceDiagramViewModel(diagram);
            VisibleDiagramType = Diagram.Type.Sequence;

            SetupBindings();
        }

        private void SetupBindings()
        {
            VisibleDiagramTypeChanged.Subscribe(newType =>
            {
                SeqDiagramVM.IsVisible = newType == Diagram.Type.Sequence;
                CommDiagramVM.IsVisible = newType == Diagram.Type.Communication;
            });
        }

        /// <summary>
        /// Switch the views from communication diagram to sequence diagram and vice versa.
        /// </summary>
        public void SwitchViews()
        {
            switch (VisibleDiagramType)
            {
                case Diagram.Type.Sequence:
                    VisibleDiagramType = Diagram.Type.Communication;
                    break;
                case Diagram.Type.Communication:
                    VisibleDiagramType = Diagram.Type.Sequence;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public DiagramDialogViewModel CreateDiagramDialogViewModel()
        {
            DiagramDialogViewModel dialog = new DiagramDialogViewModel
            {
                SelectedDiagramType = SeqDiagramVM.IsVisible ? Diagram.Type.Sequence : Diagram.Type.Communication
            };

            dialog.SelectedDiagramTypeChanged.Subscribe(newDiagType => VisibleDiagramType = newDiagType);
            VisibleDiagramTypeChanged.Subscribe(newType => dialog.SelectedDiagramType = newType);

            return dialog;
        }
    }
}
