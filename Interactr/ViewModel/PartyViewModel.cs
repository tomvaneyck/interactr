using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Framework;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The view model for a party.
    /// </summary>
    public class PartyViewModel
    {
        #region Type

        private readonly ReactiveProperty<Party.PartyType> _type = new ReactiveProperty<Party.PartyType>();

        /// <summary>
        /// Represent the type of this party.
        /// </summary>
        /// <remarks>
        /// It is impossible for a party to not have a type.
        /// </remarks>
        public Party.PartyType Type
        {
            get => _type.Value;
            private set => _type.Value = value;
        }

        /// <summary>
        /// An observable that emits the new party type when it has changed.
        /// </summary>
        public IObservable<Party.PartyType> TypeChanged => _type.Changed;

        #endregion

        #region Label

        private readonly ReactiveProperty<string> _label = new ReactiveProperty<string>();

        /// <summary> A label.
        /// <example> instance_name;class_name </example>
        /// </summary>
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

        #region CanApplyLabel

        private readonly ReactiveProperty<bool> _canApplyLabel = new ReactiveProperty<bool>();

        /// <summary>Is the label valid?</summary>
        public bool CanApplyLabel
        {
            get => _canApplyLabel.Value;
            set => _canApplyLabel.Value = value;
        }

        public IObservable<bool> CanApplyLabelChanged => _canApplyLabel.Changed;

        #endregion

        #region PartyPosition

        private ReactiveProperty<Point> _position = new ReactiveProperty<Point>();

        /// <summary>
        /// The position of the party. 
        /// </summary>
        public Point Position
        {
            get => _position.Value;
            set => _position.Value = value;
        }

        public IObservable<Point> PositionChanged => _position.Changed;

        #endregion

        public Party Party { get; }

        public PartyViewModel(Party party)
        {
            Party = party;

            // Bind the type in the viewmodel to the type in the model.
            party.TypeChanged.Subscribe(newType => Type = newType);

            // Define the label in the viewmodel to change when the label changes in the model.
            party.LabelChanged.Subscribe(newLabel => Label = newLabel);

            // Update CanApplyLabel when the label changes.
            LabelChanged.Select(Party.IsValidLabel).Subscribe(isValid => CanApplyLabel = isValid);
        }

        /// <summary>
        /// Change the party type of this party in the view model and in the model.
        /// </summary>
        public void SwitchPartyType()
        {
            // Change the type in the viewmodel.
            Type = Type == Party.PartyType.Actor ? Party.PartyType.Object : Party.PartyType.Actor;

            // Change the type in the model.
            Party.Type = Type;
        }

        /// <summary>
        /// Set the model label to the label in the viewmodel.
        /// </summary>
        public void ApplyLabel()
        {
            Party.Label = Label;
        }
    }
}