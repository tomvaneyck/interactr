using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.ViewModel.MessageStack;

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

        ///<summary>
        /// The messageView models present in the StackVM.
        /// <remarks>
        /// This is necessary because the compiler cannot derive inheritance for generic types
        /// in DerivedList which causes the compiler to complain at compileTime when specifying IReadOnlyDerivedList.
        /// </remarks>
        /// </summary>
        protected override IReadOnlyList<MessageViewModel> MessageViewModels
        {
            get => StackVM.MessageViewModels;
        }

        public SequenceDiagramViewModel(Diagram diagram) : base(diagram)
        {
            StackVM = new MessageStackViewModel(diagram);
        }
    }
}