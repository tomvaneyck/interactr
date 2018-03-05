using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Interactr.Model
{
    /// <summary>
    /// Represents an Actor or an Object in the diagram.
    /// </summary>
    public class Party
    {
        private string _label;

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
        /// Represent the type of this party.
        /// </summary>
        /// <remarks>
        /// It is impossible for a party to not have a type.
        /// </remarks>
        public PartyType Type { get; set; }

        /// <summary>
        ///  A label in the valid format.
        /// </summary>
        /// <exception cref="ArgumentException"> Throw an ArgumentException if the label has an invalid format.</exception>
        public string Label
        {
            get => _label;
            set
            {
                if (IsValidLabel(value))
                {
                    _label = value;
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
        /// a valid format is specified as follows:
        /// An optional instance_name starting with lowercase, followed by a colon and a class name starting with uppercase. 
        /// </remarks>
        /// <example> [instance_name]:class_name </example>
        /// </summary>
        /// <param name="label"> The label string.</param>
        /// <returns>A boolean indicating if it is a valid label.</returns>
        public static bool IsValidLabel(string label)
        {
            return Regex.Match(label,
                    "^(([a-z\u00C0-\u017F]{1}[a-zA-Z0-9\u00C0-\u017F]*)?:){1}([A-Z\u00C0-\u017F]{1}[a-zA-Z0-9\u00C0-\u017F]*)+$")
                .Success;
        }
    }