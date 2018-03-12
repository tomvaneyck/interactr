using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;

namespace Interactr.ViewModel
{
    /// <summary> The diagram view model for the sequence diagram </summary>
    /// <see cref="DiagramViewModel"/>
    public class CommunicationDiagramViewModel : DiagramViewModel
    {
        public CommunicationDiagramViewModel(Diagram diagram) : base(diagram)
        {
        }

        /// <summary>
        /// Add a new party at the specified point. 
        /// </summary>
        /// <param name="point"> The point on the screen where the party is added.</param>
        public void AddParty(Point point)
        {
            Party party = new Party(Party.PartyType.Actor, ValidLabel);
            Diagram.Parties.Add(party);
            PartyViewModels.First(vm => vm.Party == party).Position = point;
        }
    }
}