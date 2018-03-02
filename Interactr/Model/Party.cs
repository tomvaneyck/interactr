using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

        /// <summary>
        /// Represent the type of this party.
        /// </summary>
        /// <remarks>
        /// It is impossible for a party to not have a type.
        /// </remarks>
        public PartyType Type { get; set; }

        /// <summary> A label in the specified format.
        /// <example> instance_name;class_name </example>
        /// </summary>
        //TODO validate label before assignment.
        public string Label { get; set; }

        public bool isValidLabel(string label)
        {
            string pattern = @"(^;|^[a-z]\w*;)[A-Z]\w*";
            return Regex.IsMatch(label,pattern);
        }
    }


}