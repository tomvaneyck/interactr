using Interactr.Model;

namespace Interactr.ViewModel
{
    /// <summary>
    /// A viewModel for the activationBar.
    /// the activation bar is immutable.
    /// </summary>
    /// <remarks>There is no underlying model for the activationBar viewModel.</remarks>
    public class ActivationBarViewModel
    {
        #region party

        public Party Party { get; }

        #endregion

        #region tick

        public int StartTick { get; }

        #endregion

        #region message tick

        public int MessageTick { get; }

        #endregion

        public ActivationBarViewModel(Party party, int startTick, int messageTick)
        {
            Party = party;
            StartTick = startTick;
            MessageTick = messageTick;
        }
    }
}