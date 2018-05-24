﻿using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    public class PartyFormatStringViewModel : IFormatStringViewModel
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

        #region InstanceName

        private readonly ReactiveProperty<string> _instanceName = new ReactiveProperty<string>();

        private string InstanceName
        {
            get => _instanceName.Value;
            set => _instanceName.Value = value;
        }

        #endregion

        #region ClassName

        private readonly ReactiveProperty<string> _className = new ReactiveProperty<string>();

        private string ClassName
        {
            get => _className.Value;
            set => _className.Value = value;
        }

        #endregion

        public PartyFormatStringViewModel()
        {
            // Update the instanceName and the className when the label changes.
            TextChanged.Where(t => t != null).Subscribe(newLabelText =>
                {
                    var splitText = newLabelText.Split(':');

                    // If the text contains a : and thus can be split up in an 
                    // instanceName and a className
                    if (splitText.Length >= 2)
                    {
                        InstanceName = splitText[0];
                        var className = "";

                        for (int i = 1; i < splitText.Length; i++)
                        {
                            className += splitText[i];
                        }

                        ClassName = className;
                    }
                    else
                    {
                        // If the text cannot be split up in instanceName and className 
                        // than set ClassName to null and the instanceName to the whole text.
                        InstanceName = newLabelText;
                        ClassName = null;
                    }
                }
            );

            // Update the text of the label if the instanceName or the className changes.
            _instanceName.Changed.MergeEvents(_className.Changed)
                .Subscribe(_ =>
                {
                    if (ClassName == null)
                    {
                        Text = InstanceName;
                    }
                    else
                    {
                        Text = InstanceName + ":" + ClassName;
                    }
                });
        }

        public bool HasValidLabel()
        {
            return Party.IsValidLabel(Text);
        }
    }
}