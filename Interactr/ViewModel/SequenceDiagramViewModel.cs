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
    /// <inheritdoc cref="DiagramViewModel"/>
    public class SequenceDiagramViewModel : DiagramViewModel
    {
        /// <summary>
        /// Add a party to the partyViewModels.
        /// </summary>
        public void AddParty()
        {
            Debug.WriteLine("Add Party.");
            PartyViewModels.Add(new PartyViewModel(new Party(Party.PartyType.Actor, ValidLabel)));
        }
    }
}