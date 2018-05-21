﻿using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Forms;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View
{
    /// <summary>
    /// A view for a communication diagram message.
    /// </summary>
    public class CommunicationDiagramMessageView : UIElement
    {
        #region CommunicationDiagramMessageViewModel

        private readonly ReactiveProperty<MessageViewModel> _viewModel =
            new ReactiveProperty<MessageViewModel>();

        public MessageViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<MessageViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        /// <summary>
        /// The start point of the message arrow.
        /// </summary>
        /// <remarks>
        ///  This is an accessor for the endpoint of the private variable _arrow.
        /// </remarks>
        public Point ArrowStartPoint
        {
            get => _arrow.StartPoint;
            set => _arrow.StartPoint = value;
        }

        /// <summary>
        /// The end point of the message arrow.
        /// </summary>
        /// <remarks>
        /// This is an accessor for the endpoint of the private variable _arrow.
        /// </remarks>
        public Point ArrowEndPoint
        {
            get => _arrow.EndPoint;
            set => _arrow.EndPoint = value;
        }

        /// <summary>
        /// The label with messageNumber view of the message.
        /// </summary>
        public LabelWithMessageNumberView LabelWithMessageNumberView { get; } = new LabelWithMessageNumberView();

        private readonly ArrowView _arrow = new ArrowView();

        public CommunicationDiagramMessageView(MessageViewModel viewModel)
        {
            IsVisibleToMouse = false;

            ViewModel = viewModel;

            Children.Add(_arrow);
            Children.Add(LabelWithMessageNumberView);

            // Change the size of the arrow views.
            WidthChanged.Subscribe(newWidth => _arrow.Width = newWidth);
            HeightChanged.Subscribe(newHeight => _arrow.Height = newHeight);

            // Update the label on a change.
            ViewModel.LabelChanged.Subscribe(_ => LabelWithMessageNumberView.LabelView.Text = ViewModel.Label);

            LabelWithMessageNumberView.LabelView.TextChanged.Subscribe(text =>
            {
                if (ViewModel != null)
                {
                    ViewModel.Label = text;
                }
            });

            // Update the messageNumber on a change
            ViewModel.MessageNumberChanged.Subscribe(m =>
            {
                LabelWithMessageNumberView.SetMessageNumber( m);
            });

            LabelWithMessageNumberView.PreferredHeightChanged.Subscribe(h => LabelWithMessageNumberView.Height = h);
            LabelWithMessageNumberView.PreferredWidthChanged.Subscribe(w => LabelWithMessageNumberView.Width = w);

            // Put the label under the arrow.
            Observable.CombineLatest(
                _arrow.StartPointChanged,
                _arrow.EndPointChanged
            ).Subscribe(p =>
            {
                // Get the leftmost point
                Point start = p[0].X < p[1].X ? p[0] : p[1];
                // Get the rightmost point
                Point end = p[0].X > p[1].X ? p[0] : p[1];
                // Get the vector from the leftmost to the rightmost point.
                Point diff = end - start;
                // Start the text at a third of the distance between the points. Looks good enough for now.
                Point textPos = start + new Point(diff.X / 2, diff.Y / 2);

                // Set the labelMessageNumberView position
                LabelWithMessageNumberView.Position =
                    new Point(textPos.X, textPos.Y);
            });
        }

        /// <summary>
        /// Observe the position of the party given by the party selector.
        /// </summary>
        /// <param name="partySelector">The selector of the party.</param>
        /// <returns>An observable with the partyview of the party where the position changed.</returns>
        private IObservable<PartyView> ObservePartyPosition(
            Func<MessageViewModel, IObservable<Party>> partySelector)
        {
            // Select the latest parent view
            return ParentChanged.OfType<CommunicationDiagramView>().Select(parent =>
                // and the latest viewmodel
                    ViewModelChanged.Where(vm => vm != null).Select(vm =>
                        // and the latest matching sender
                            partySelector(vm).Where(party => party != null).Select(targetParty =>
                            {
                                // and listen for the position changes of its view.
                                return parent.PartyViewsDragPanel.PartyViews.ObserveWhere(
                                    party => party.PositionChanged,
                                    party => party.ViewModel.Party == targetParty).Select(e => e.Element);
                            }).Switch()
                    ).Switch()
            ).Switch();
        }
    }
}