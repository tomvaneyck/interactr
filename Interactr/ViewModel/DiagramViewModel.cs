using System;
using Interactr.Reactive;
using Interactr.View.Controls;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The ViewModel for a diagram.
    /// </summary>
    /// <remarks> A view model represents the data you want to display on your view
    /// and is responsible for interaction with the data objects from the model.</remarks>
    public abstract class DiagramViewModel
    {
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

        public IObservable<(PartyViewModel, int)> PartyViewOnAdd => PartyViewModels.OnAdd;

        public IObservable<(PartyViewModel, int)> PartyViewModelOnDelete => PartyViewModels.OnDelete;

        #endregion
    }
}