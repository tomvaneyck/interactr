using System;
using System.Windows.Forms;
using Interactr.Reactive;

namespace Interactr.Model
{
    /// <summary>
    /// Represents an Actor or an Object in the diagram.
    /// </summary>
    public class Party
    {
        public Party(PartyType type, string label)
        {
            Type = type;
            Label = label;
        }

        /// <summary>
        /// Enum that represents the type of a Party.
        /// </summary>
        public enum PartyType
        {
            Actor,
            Object
        }
        
        #region Type
        private readonly ReactiveProperty<PartyType> _type = new ReactiveProperty<PartyType>();

        /// <summary>
        /// Represent the type of this party.
        /// </summary>
        /// <remarks>
        /// It is impossible for a party to not have a type.
        /// </remarks>
        public PartyType Type
        {
            get => _type.Value;
            set => _type.Value = value;
        }

        /// <summary>
        /// An observable that emits the new party type when it has changed.
        /// </summary>
        public IObservable<PartyType> TypeChanged => _type.Changed;
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
    }
}