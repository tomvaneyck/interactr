using System;
using System.Diagnostics;
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

        public bool IsVisible
        {
            get => _isVisible.Value;
            set => _isVisible.Value = value;
        }

        public IObservable<bool> IsVisibleChanged => _isVisible.Changed;

        #endregion

        #region partyViewModels

        public ReactiveList<PartyViewModel> PartyViewModels { get; } = new ReactiveList<PartyViewModel>();

        public IObservable<(PartyViewModel, int)> PartyViewModelOnAdd => PartyViewModels.OnAdd;

        public IObservable<(PartyViewModel, int)> PartyViewModelOnDelete => PartyViewModels.OnDelete;

        #endregion

        public void AddParty(Point point)
        {
            PartyViewModels.Add(new PartyViewModel(new Party(Party.PartyType.Actor, ValidLabel))
            {
                Position = point
            });
        }
    }
}