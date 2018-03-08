﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The ViewModel for the communication diagram.
    /// </summary>
    /// <remarks> A view model represents the data you want to display on your view
    /// and is responsible for interaction with the data objects from the model.</remarks>
    public class CommunicationDiagramViewModel
    {
        #region PartyViewModels

        public ReactiveList<PartyViewModel> PartyViewModels { get; } = new ReactiveList<PartyViewModel>();

        #endregion

        #region IsVisible

        private readonly ReactiveProperty<bool> _isVisible = new ReactiveProperty<bool>();


        public bool IsVisible
        {
            get => _isVisible.Value;
            set => _isVisible.Value = value;
        }

        public IObservable<bool> IsVisibleChanged => _isVisible.Changed;

        #endregion
    }
}