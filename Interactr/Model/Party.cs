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
        /// <example> [instance_name]:class_name </example>
        /// </summary>
        /// <remarks>
        /// The label is only accepted if the optional instance_name starts with lowercase,
        /// has one colon followed by a class_name starting with uppercase.
        /// The label is thus only accepted if it follows on of the following patterns:
        /// <list type="bullet">
        /// <item>test:Approved</item>
        /// <item>:Approved</item>
        /// </list>
        /// and rejected when it follows one of the following patterns:
        /// <list type="bullet">
        /// <item>Test:Rejected</item>
        /// <item>test:rejected</item>
        /// <item>test:</item>
        /// </list>
        /// </remarks>
        //TODO validate label before assignment.
        public string Label { get; set; }

        public static bool IsValidLabel(string label)
        {
            if (Regex.Match(label, "^(([a-z\u00C0-\u017F]{1}[a-zA-Z0-9\u00C0-\u017F]*)?:){1}([A-Z\u00C0-\u017F]{1}[a-zA-Z0-9\u00C0-\u017F]*)+$").Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}