using Interactr.Model;

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

        public DiagramEditorViewModel(Diagram diagram)
        {
            Diagram = diagram;
            CommDiagramVM = new CommunicationDiagramViewModel(diagram);
            SeqDiagramVM = new SequenceDiagramViewModel(diagram)
            {
                IsVisible = true
            };
        }

        /// <summary>
        /// Switch the views from communication diagram to sequence diagram and vice versa.
        /// </summary>
        public void SwitchViews()
        {
            if (CommDiagramVM.IsVisible)
            {
                CommDiagramVM.IsVisible = false;
                SeqDiagramVM.IsVisible = true;
            }
            else //If SeqDiagramVM or neither is visible
            {
                CommDiagramVM.IsVisible = true;
                SeqDiagramVM.IsVisible = false;
            }
        }
    }
}
