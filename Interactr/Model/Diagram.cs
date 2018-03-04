using System.Collections.Generic;
using Interactr.Reactive;

namespace Interactr.Model
{
    /// <summary>
    /// Contain all the elements present in the UML diagram.
    /// </summary>
    public class Diagram
    {
        public Diagram()
        {
            Messages = new ReactiveList<Message>();
            Parties = new ReactiveList<Party>();
        }

        /// <summary>
        /// IList of parties present in the diagram.
        /// </summary>
        public IList<Party> Parties { get;}

        /// <summary>
        /// List of messages present in the diagram.
        /// </summary>
        public IList<Message> Messages { get; }
    }
}