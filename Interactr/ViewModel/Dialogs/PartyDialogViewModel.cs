using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel.Dialogs
{
    public class PartyDialogViewModel
    {
        public PartyFormatStringViewModel Label { get; } = new PartyFormatStringViewModel();
        
        #region PartyType

        private readonly ReactiveProperty<Party.PartyType> _partyType = new ReactiveProperty<Party.PartyType>();

        public Party.PartyType PartyType
        {
            get => _partyType.Value;
            set => _partyType.Value = value;
        }

        public IObservable<Party.PartyType> PartyTypeChanged => _partyType.Changed;

        #endregion
    }
}
