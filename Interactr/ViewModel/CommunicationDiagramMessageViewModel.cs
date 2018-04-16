using System;
using Interactr.Model;

namespace Interactr.ViewModel
{
    public class CommunicationDiagramMessageViewModel : MessageViewModel
    {

        /// <summary>
        /// The message number of the message on it's own nesting level.
        /// For example, a message has an 
        /// </summary>
        private int MessageNumber {get; set; }

        public CommunicationDiagramMessageViewModel(Message message) : base(message)
        {
        }
    }
}