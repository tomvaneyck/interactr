using System.Collections.Generic;

namespace Interactr.Model
{
    /// <summary>
    /// Contain all the elements present in the UML diagram.
    /// </summary>
    public class Diagram
    {
        public Diagram()
        {
            Parties = new List<Party>();
            Messages = new List<Message>();
        }

        /// <summary>
        /// IList of parties present in the diagram.
        /// </summary>
        public IList<Party> Parties { get; set; }

        /// <summary>
        /// List of messages present in the diagram.
        /// </summary>
        public IList<Message> Messages { get; set; }
    }
}