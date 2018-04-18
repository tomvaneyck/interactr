using Interactr.Model;

namespace Interactr.ViewModel
{
    /// <summary>
    /// A view model for the message in communication diagram.
    /// </summary>
    public class CommunicationDiagramMessageViewModel : MessageViewModel
    {
        public CommunicationDiagramMessageViewModel(Message message) : base(message)
        {
        }
    }
}