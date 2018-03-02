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
        private readonly ReactiveProperty<PartyType> _type = new ReactiveProperty<PartyType>();
        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

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

        /// <summary>
        /// A stream of changed types.
        /// </summary>
        public IObservable<PartyType> TypeChanged => _type.Changed;

        /// <summary>
        /// A stream of changed labels.
        /// </summary>
        public IObservable<string> labelChanged => _label.Changed;

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

        /// <summary> A label in the specified format.
        /// <example> instance_name;class_name </example>
        /// </summary>
        //TODO validate label before assignment.
        public string Label
        {
            get => _label.Value;
            set => _label.Value = value;
        }
    }
}