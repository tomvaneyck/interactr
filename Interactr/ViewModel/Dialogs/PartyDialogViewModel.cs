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
        #region ClassName

        private readonly ReactiveProperty<string> _className = new ReactiveProperty<string>();

        public string ClassName
        {
            get => _className.Value;
            set => _className.Value = value;
        }

        public IObservable<string> ClassNameChanged => _className.Changed;

        #endregion
        
        #region InstanceName

        private readonly ReactiveProperty<string> _instanceName = new ReactiveProperty<string>();

        public string InstanceName
        {
            get => _instanceName.Value;
            set => _instanceName.Value = value;
        }

        public IObservable<string> InstanceNameChanged => _instanceName.Changed;

        #endregion
        
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
