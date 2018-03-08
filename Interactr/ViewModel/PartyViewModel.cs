using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The view model for a party.
    /// </summary>
    public class PartyViewModel
    {
        #region Type
        private readonly ReactiveProperty<Party.PartyType> _type = new ReactiveProperty<Party.PartyType>();

        /// <summary>
        /// Represent the type of this party.
        /// </summary>
        /// <remarks>
        /// It is impossible for a party to not have a type.
        /// </remarks>
        public Party.PartyType Type
        {
            get => _type.Value;
            set => _type.Value = value;
        }

        /// <summary>
        /// An observable that emits the new party type when it has changed.
        /// </summary>
        public IObservable<Party.PartyType> TypeChanged => _type.Changed;
        #endregion

        #region Label
        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

        /// <summary> A label in the specified format.
        /// <example> instance_name;class_name </example>
        /// </summary>
        //TODO validate label before assignment.
        public string Label
        {
            get => _label.Value;
            set => _label.Value = value;
        }

        /// <summary>
        /// An observable that emits the new label when it has changed.
        /// </summary>
        public IObservable<string> LabelChanged => _label.Changed;
        #endregion

        /// <summary>
        /// Change the party type of this party. 
        /// </summary>
        public void SwitchPartyType()
        {
            Type = Type == Party.PartyType.Actor ? Party.PartyType.Object : Party.PartyType.Actor;
        }
    }
}
