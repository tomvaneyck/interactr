using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.ViewModel
{
    public class LifeLineViewModel
    {
        public MessageStackViewModel MessageStackVM { get; }
        public PartyViewModel PartyVM { get; }

        public LifeLineViewModel(MessageStackViewModel messageStackViewModel, PartyViewModel party)
        {
            MessageStackVM = messageStackViewModel;
            PartyVM = party;
        }
    }
}