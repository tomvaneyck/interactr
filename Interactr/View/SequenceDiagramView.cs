﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    /// <summary>
    /// The view for the sequence diagram.
    /// </summary>
    public class SequenceDiagramView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<SequenceDiagramViewModel> _viewModel =
            new ReactiveProperty<SequenceDiagramViewModel>();

        public SequenceDiagramViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<SequenceDiagramViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public IReadOnlyReactiveList<SequenceDiagramColumnView> ColumnViews { get; private set; }

        /// View for the message currently being dragged by the user.
        private readonly ArrowView _pendingMessageView = new ArrowView();

        public SequenceDiagramView()
        {
            // The visibility of this view is to be set to the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged)
                .Subscribe(isVisible => { this.IsVisible = isVisible; });

            SetupPartyColumns();
            SetupMessages();
            SetupPendingMessage();
        }
        private void SetupPartyColumns()
        {
            // Create a horizontal stackpanel
            StackPanel stackPanel = new StackPanel
            {
                StackOrientation = Orientation.Horizontal
            };
            Children.Add(stackPanel);

            // Create a list of party views based on the party viewmodels.
            ColumnViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.PartyViewModels)
                .CreateDerivedListBinding(partyVM => new SequenceDiagramColumnView(this, partyVM))
                .ResultList;

            // Automatically add and remove columns to the stackpanel.
            ColumnViews.OnAdd.Subscribe(e => stackPanel.Children.Insert(e.Index, e.Element));
            ColumnViews.OnDelete.Subscribe(e => stackPanel.Children.RemoveAt(e.Index));
        }

        private void SetupMessages()
        {
            // Create a list of message views based on the message viewmodels.
            IReadOnlyReactiveList<SequenceDiagramMessageView> messageViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.StackVM.MessageViewModels)
                .CreateDerivedListBinding(vm => new SequenceDiagramMessageView { ViewModel = vm }).ResultList;

            // Automatically add and remove message views to Children.
            messageViews.OnAdd.Subscribe(e => Children.Add(e.Element));
            messageViews.OnDelete.Subscribe(e => Children.Remove(e.Element));
        }

        private void SetupPendingMessage()
        {
            // Hide the pending message view if there is no pending message.
            ViewModelChanged
                .ObserveNested(vm => vm.StackVM.PendingInvokingMessageVMChanged)
                .Select(vm => vm != null)
                .Subscribe(hasPendingMessage => _pendingMessageView.IsVisible = hasPendingMessage);

            // Set the correct start and endpoint for the pending message view.
            ViewModelChanged
                .ObserveNested(vm => vm.StackVM.PendingInvokingMessageVMChanged)
                .ObserveNested(vm => vm.SenderActivationBarChanged)
                .Subscribe(sender =>
                {
                    // Find the senders lifeline view.
                    var lifeLine = ColumnViews
                        .First(c => c.PartyView.ViewModel.Party == sender.Party)
                        .LifeLineView;

                    // Get the correct activation bar, if any.
                    var bar = lifeLine.ActivationBarViews.FirstOrDefault(b => b.ViewModel == sender);

                    if (bar == null)
                    {
                        // Get the startpoint of the pending message arrow, relative to the lifeline.
                        Point pointOnLifeline = new Point(
                            lifeLine.Width / 2,
                            (LifeLineView.TickHeight * ViewModel.StackVM.PendingInvokingMessageVM.Tick) - (LifeLineView.TickHeight / 2)
                        );

                        // Translate the point to this view, and assign it.
                        _pendingMessageView.StartPoint = lifeLine.TranslatePointTo(this, pointOnLifeline);
                    }
                    else
                    {
                        // Calculate how far into this activation the pending message is sent.
                        int relativeTick = ViewModel.StackVM.PendingInvokingMessageVM.Tick - (bar?.ViewModel.StartTick ?? 0);

                        // Get the startpoint of the pending message arrow, relative to the activation bar.
                        Point pointOnBar = new Point(
                            bar.Width,
                            (LifeLineView.TickHeight * relativeTick) - (LifeLineView.TickHeight / 2)
                        );

                        // Translate the point to this view, and assign it.
                        _pendingMessageView.StartPoint = bar.TranslatePointTo(this, pointOnBar);
                    }
                });

            Children.Add(_pendingMessageView);
        }

        protected override bool OnMouseEventPreview(MouseEventData eventData)
        {
            // Update the endpoint position of the pending message when the mouse is dragged around the view.
            if (eventData.Id == MouseEvent.MOUSE_DRAGGED && ViewModel?.StackVM.PendingInvokingMessageVM != null)
            {
                _pendingMessageView.EndPoint = new Point(eventData.MousePosition.X, _pendingMessageView.StartPoint.Y);
            }

            return base.OnMouseEventPreview(eventData);
        }
    }

    /// <summary>
    /// The sequence diagram column view.
    /// Contains the party view and a corresponding lifeline.
    /// </summary>
    public class SequenceDiagramColumnView : AnchorPanel
    {
        public PartyView PartyView { get; }
        public LifeLineView LifeLineView { get; }

        public SequenceDiagramColumnView(SequenceDiagramView parent, PartyViewModel partyVM)
        {
            // Create the party view and add it to this column view.
            PartyView = new PartyView
            {
                ViewModel = partyVM
            };
            AnchorsProperty.SetValue(PartyView, Anchors.Left | Anchors.Top | Anchors.Right);
            Children.Add(PartyView);

            // Create the lifeline view and add it to this column view.
            LifeLineView = new LifeLineView();
            MarginsProperty.SetValue(LifeLineView, new Margins(0, PartyView.PreferredHeight, 0, 0));
            Children.Add(LifeLineView);
            
            // Setup viewmodel subscription.
            parent.ViewModelChanged.Select(vm => vm.StackVM)
                .Subscribe(stackVM => LifeLineView.ViewModel = stackVM.CreateLifeLineForParty(partyVM));
        }
    }
}