namespace Interactr.ViewModel
{
    /// <summary>
    /// A life line view model for the lifeline in sequence diagram.
    /// </summary>
    public class LifeLineViewModel
    {
        /// <summary>
        /// The message stack view model associated with the lifeline.
        /// </summary>
        public MessageStackViewModel MessageStackVM { get; }

        /// <summary>
        /// The party view model associated with the lifeline.
        /// </summary>
        public PartyViewModel PartyVM { get; }

        public LifeLineViewModel(MessageStackViewModel messageStackViewModel, PartyViewModel party)
        {
            MessageStackVM = messageStackViewModel;
            PartyVM = party;
        }
    }
}