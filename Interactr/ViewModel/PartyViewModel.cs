using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
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
            private set => _type.Value = value;
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

        #region Party

        public Party Party { get; }

        #endregion

        public PartyViewModel(Party party)
        {
            Party = party;

            // Define the type in the viewmodel to be changed when the type changes in the model.
            party.TypeChanged.Subscribe(newType => Type = newType);
            
            // Define the label in the viewmodel to change when the label changes in the model.
            party.LabelChanged.Subscribe(newLabel => Label = newLabel);
        }

        /// <summary>
        /// Change the party type of this party in the view model and in the model.
        /// </summary>
        public void SwitchPartyType()
        {
            // Change the type in the viewmodel.
            Type = Type == Party.PartyType.Actor ? Party.PartyType.Object : Party.PartyType.Actor;

            // Change the type in the model
            Party.Type = Type;
        }
    }
}