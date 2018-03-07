using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The ViewModel for the mainView.
    /// </summary>
    /// <remarks> A view model represents the data you want to display on your view
    /// and is responsible for interaction with the data objects from the model.</remarks>
    public class MainViewModel
    {
        public CommunicationDiagramViewModel CommDiagramVM { get; } = new CommunicationDiagramViewModel();
        public SequenceDiagramViewModel SeqDiagramVM { get; } = new SequenceDiagramViewModel();
        public Diagram Diagram { get; private set; }

        public MainViewModel(Diagram diagram)
        {
            Diagram = diagram;
            SeqDiagramVM.IsVisible = true;
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