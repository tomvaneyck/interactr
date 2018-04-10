using System;
using System.Reactive.Linq;
using Interactr.View.Controls;
using Interactr.ViewModel;

namespace Interactr.View
{
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

            // Set up viewmodel subscription.
            parent.ViewModelChanged.Select(vm => vm.StackVM)
                .Subscribe(stackVM => LifeLineView.ViewModel = stackVM.CreateLifeLineForParty(partyVM));

            IsVisible = parent.IsVisible;
            parent.IsVisibleChanged.Subscribe(newVisible => IsVisible = newVisible);
        }
    }
}