using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary> The diagram view model for the sequence diagram </summary>
    /// <see cref="DiagramViewModel"/>
    public class SequenceDiagramViewModel : DiagramViewModel
    {
        /// <summary>
        /// The message stack view model associated with the diagram.
        /// </summary>
        public MessageStackViewModel StackVM { get; }

        public SequenceDiagramViewModel(Diagram diagram) : base(diagram)
        {
            StackVM = new MessageStackViewModel(diagram);
        }
    }
}