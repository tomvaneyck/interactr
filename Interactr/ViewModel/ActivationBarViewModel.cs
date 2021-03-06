﻿using Interactr.Model;

namespace Interactr.ViewModel
{
    /// <summary>
    /// A viewModel for the activationBar.
    /// <remarks>The activation bar is immutable after creation.</remarks>
    /// </summary>
    /// <remarks>There is no underlying model for the activationBar viewModel.</remarks>
    public class ActivationBarViewModel
    {
        /// <summary>
        /// The party which is activated.
        /// </summary>
        public Party Party { get; }

        /// <summary>
        /// The timestamp at which this activation starts.
        /// </summary>
        public int StartTick { get; }

        /// <summary>
        /// The timestamp at which this activation ends.
        /// </summary>
        public int EndTick { get; }

        /// <summary>
        /// The level of nesting of this activation bar.
        /// </summary>
        public int Level { get; }

        public ActivationBarViewModel(Party party, int startTick, int endTick, int level)
        {
            Party = party;
            StartTick = startTick;
            EndTick = endTick;
            Level = level;
        }
    }
}