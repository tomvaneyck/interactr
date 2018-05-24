using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary>
    /// A container for the label of an invocation message, also containing
    /// the methodName and methodArguments that get parsed out of the text.
    /// </summary>
    public class InvocationFormatStringViewModel : IFormatStringViewModel
    {
        #region Text

        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        /// <see cref="Text"/>
        public string Text
        {
            get => _text.Value;
            set => _text.Value = value;
        }

        public IObservable<string> TextChanged => _text.Changed;

        #endregion

        #region MethodName

        private readonly ReactiveProperty<string> _methodName = new ReactiveProperty<string>();

        private string MethodName
        {
            get => _methodName.Value;
            set => _methodName.Value = value;
        }

        #endregion

        #region MethodArguments

        private readonly ReactiveProperty<List<string>> _methodArguments = new ReactiveProperty<List<string>>();

        private List<string> MethodArguments
        {
            get => _methodArguments.Value;
            set => _methodArguments.Value = value;
        }

        #endregion

        public InvocationFormatStringViewModel()
        {
            // Update the methodName and the method arguments when the label in the viewmodel changes.
            TextChanged.Subscribe(newLabelText =>
                {
                    var newMethodName = InvocationLabelParser.RetrieveMethodNameFromLabel(newLabelText);
                    var newMethodArguments = InvocationLabelParser.RetrieveArgumentsFromLabel(newLabelText);

                    MethodName = newMethodName;
                    if (newMethodArguments != null)
                    {
                        MethodArguments = newMethodArguments.ToList();
                    }
                }
            );

            // Update the label on a change in the methodName or methodArguments.
            _methodName.Changed.MergeEvents(_methodArguments.Changed).Subscribe(_ =>
            {
                var newLabel = MethodName;
                newLabel += "(";

                if (MethodArguments != null)
                {
                    foreach (var arg in MethodArguments)
                    {
                        newLabel += arg + ",";
                    }
                }

                if (newLabel[newLabel.Length - 1] == ',')
                {
                    newLabel = newLabel.Substring(0, newLabel.Length - 1);
                }

                newLabel += ")";
                Text = newLabel;
            });
        }

        /// <summary>
        /// Indicate wether the text of this  invocation messageLabel is a valid label.
        /// </summary>
        /// <returns></returns>
        public bool HasValidText()
        {
            return Message.IsValidInvocationLabel(Text);
        }
    }
}