using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Message = Interactr.Model.Message;

namespace Interactr
{
    public static class InvocationLabelParser
    {
        /// <summary>
        /// Return the method name out of an invocation label when given a valid label.
        /// Return null if the label does not contain an opening bracket
        /// </summary>
        /// <param name="label"> The label of an invocation message.</param>
        /// <returns></returns>
        public static string RetrieveMethodNameFromLabel(string label)
        {
            // Split the label on opening bracket.
            var splitLabel = label.Split('(');
            return splitLabel.Length == 0 ? null : splitLabel[0];
        }

        /// <summary>
        /// Return the list of arguments of an invocation label when given a valid label.
        /// Return null if the label does not contain an opening bracket and closing bracket
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static List<string> RetrieveArgumentsFromLabel(string label)
        {
            // Get the arguments enclosed in brackets
            var argsMatch = Regex.Match(label, "(.*)");
            if (!argsMatch.Success)
            {
                return null;
            }

            // Split the arguments on ",".
            var args = argsMatch.Value.Split(',');

            return args.ToList();
        }
    }
}