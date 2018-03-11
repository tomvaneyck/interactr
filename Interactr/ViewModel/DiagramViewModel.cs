using System;
using System.Diagnostics;
using System.Windows.Forms;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The ViewModel for a diagram.
    /// </summary>
    /// <remarks> A view model represents the data you want to display on your view
    /// and is responsible for interaction with the data objects from the model.</remarks>
    public abstract class DiagramViewModel
    {
        protected const string ValidLabel = "instanceName:ClassName";

        #region IsVisible

        private readonly ReactiveProperty<bool> _isVisible = new ReactiveProperty<bool>();

        /// <summary>
        /// Indicate if the diagramModel is visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible.Value;
            set => _isVisible.Value = value;
        }

        /// <summary>
        /// Observable that emits the new IsVisible value when it is changed.
        /// </summary>
        public IObservable<bool> IsVisibleChanged => _isVisible.Changed;

        #endregion

        /// <summary>
        /// The underlying diagram associated with the diagram view model.
        /// </summary>
        /// <remarks>The underlying diagram can not be changed after the diagramViewModel construction.</remarks>
        public Diagram Diagram { get; }

        /// <summary>
        /// The partyViewModels included in this diagram view model.
        /// </summary>
        public ReactiveList<PartyViewModel> PartyViewModels { get; }

        protected DiagramViewModel(Diagram diagram)
        {
            Diagram = diagram;
            PartyViewModels = Diagram.Parties.CreateDerivedList(party => new PartyViewModel(party)).ResultList;
        }

        /// <summary>
        /// Add a new party at the specified point. 
        /// </summary>
        /// <param name="point"> The point on the screen where the party is added.</param>
        public void AddParty(Point point)
        {
            Party party = new Party(Party.PartyType.Actor, ValidLabel);
            Diagram.Parties.Add(party);
            foreach (var partyViewModel in PartyViewModels)
            {
                if (partyViewModel.Party == party)
                {
                    partyViewModel.Position = point;
                }
            }
        }
    }
}