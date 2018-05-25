using System;
using System.Reactive;
using System.Reactive.Linq;
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

        /// <summary> A label.
        /// <example> instance_name;class_name </example>
        /// </summary>
        public PartyFormatStringViewModel Label { get; set; }

        /// <summary>
        /// An observable that emits a default unit if something
        /// in the Formatted string label changes.
        /// </summary>
        public IObservable<Unit> LabelChanged => Label.FormatStringChanged;

        #endregion

        /// <summary>
        /// Indicates if the label of the party view is in edit mode.
        /// </summary>
        /// <remarks>
        /// This property gets set in the party view of this party view model.
        /// It maintains a binding so that the view model is aware of changes in the view.
        /// </remarks>
        public bool LabelInEditMode { get; set; }

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

            // Create a new party formatted string view model
            Label = new PartyFormatStringViewModel();

            // Bind the type in the viewmodel to the type in the model.
            party.TypeChanged.Subscribe(newType => Type = newType);

            // Define the label in the viewmodel to change when the label changes in the model.
            party.LabelChanged.Subscribe(newLabel =>
            {
                if (!LabelInEditMode)
                {
                    Label.Text = newLabel; 
                }
            });
            // Update CanApplyLabel when the label changes.
            Label.TextChanged.Select(Party.IsValidLabel).Subscribe(isValid => CanApplyLabel = isValid);
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
            Party.Label = Label.Text;
        }
    }
}