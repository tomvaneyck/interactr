using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Reactive;
using NUnit.Framework.Internal;

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

        public string InstanceName
        {
            get => _instanceName.Value;
            set => _instanceName.Value = value;
        }

        public IObservable<string> InstanceNameChanged => _instanceName.Changed;

        #endregion

        #region ClassName

        private readonly ReactiveProperty<string> _className = new ReactiveProperty<string>();

        public string ClassName
        {
            get => _className.Value;
            set => _className.Value = value;
        }

        public IObservable<string> ClassNameChanged => _className.Changed;

        #endregion

        public IObservable<Unit> FormatStringChanged { get; }

        private bool _isUpdating;

        public PartyFormatStringViewModel()
        {
            // Set the observables for a change to the formatString
            FormatStringChanged = TextChanged.MergeEvents(InstanceNameChanged, ClassNameChanged);
            
            // Update the instanceName and the className when the label changes.
            TextChanged.Where(t => t != null).Subscribe(newLabelText =>
            {
                if (!_isUpdating)
                {
                    _isUpdating = true;

                    UpdatePartyPropertiesFromLabel();

                    _isUpdating = false;
                }
            });

            // Update the text of the label if the instanceName or the className changes.
            InstanceNameChanged.MergeEvents(ClassNameChanged).Subscribe(_ =>
            {
                if (!_isUpdating)
                {
                    _isUpdating = true;

                    UpdateLabelFromPartyProperties();

                    _isUpdating = false;
                }
            });
        }

        private void UpdatePartyPropertiesFromLabel()
        {
            Debug.WriteLine($"{this.GetHashCode()}: UpdatePartyPropertiesFromLabel - {Text}");
            var splitText = Text.Split(':');

            // If the text contains a : and thus can be split up in an 
            // instanceName and a className
            if (splitText.Length >= 2)
            {
                InstanceName = splitText[0];
                ClassName = string.Join(":", splitText.Skip(1));
            }
            else
            {
                // If the text cannot be split up in instanceName and className 
                // than set ClassName to null and the instanceName to the whole text.
                InstanceName = Text;
                ClassName = null;
            }
            Debug.WriteLine($"   -> {InstanceName} {ClassName}");
        }

        private void UpdateLabelFromPartyProperties()
        {
            Debug.WriteLine($"{this.GetHashCode()}: UpdateLabelFromPartyProperties - {InstanceName} {ClassName}");
            if (ClassName == null)
            {
                Text = InstanceName;
            }
            else
            {
                Text = InstanceName + ":" + ClassName;
            }
            Debug.WriteLine($"   -> {Text}");
        }

        public bool HasValidText()
        {
            return Party.IsValidLabel(Text);
        }
    }
}