using System.Collections.Generic;
using Interactr.Reactive;
using System.Linq;
using System;

namespace Interactr.Model
{
    /// <summary>
    /// Contain all the elements present in the UML diagram.
    /// </summary>
    public class Diagram
    {
        public Diagram()
        {
            Messages = new ReactiveArrayList<Message>();
            Parties = new ReactiveArrayList<Party>();

            Parties.OnDelete.Subscribe(e =>
            {
                var messagesToBeRemoved = Messages
                    .Where(message => message.Sender == e.Element || message.Receiver == e.Element);
                Messages.RemoveAll(messagesToBeRemoved);
            });
        }

        /// <summary>
        /// IList of parties present in the diagram.
        /// </summary>
        public ReactiveList<Party> Parties { get;}

        /// <summary>
        /// List of messages present in the diagram.
        /// </summary>
        public ReactiveList<Message> Messages { get; }
    }
}