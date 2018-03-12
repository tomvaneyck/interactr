using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Interactr.Reactive;

namespace Interactr.Model
{
    /// <summary>
    /// Represent an Actor or an Object in the diagram.
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

        /// <summary>
        ///  A label in the valid format.
        /// </summary>
        /// <exception cref="ArgumentException"> Throw an ArgumentException if the label has an invalid format.</exception>
        public string Label
        {
            get => _label.Value;
            set
            {
                if (IsValidLabel(value))
                {
                    _label.Value = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// Return True if the given label has a valid format.
        /// <remarks>
        /// A valid format is specified as follows:
        /// An optional instance_name starting with lowercase, followed by a colon and a class name starting with uppercase.
        /// </remarks>
        /// <example> [instance_name]:Class_name </example>
        /// </summary>
        /// <param name="label"> The label string.</param>
        /// <returns>A boolean indicating if it is a valid label.</returns>
        public static bool IsValidLabel(string label)
        {
            return label != null && Regex.Match(label,
                    "^(([a-z\u00C0-\u017F]{1}[a-zA-Z0-9\u00C0-\u017F]*)?:){1}([A-Z\u00C0-\u017F]{1}[a-zA-Z0-9\u00C0-\u017F]*)+$")
                .Success;
        }

        /// <summary>
        /// An observable that emits the new label when it has changed.
        /// </summary>
        public IObservable<string> LabelChanged => _label.Changed; 
        #endregion

    }
}
