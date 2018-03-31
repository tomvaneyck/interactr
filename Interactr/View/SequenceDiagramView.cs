using System;
using System.Linq;
using System.Reactive.Linq;
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

        public SequenceDiagramView()
        {
            // Define the visibility of this view to be set to the visibility of the latest viewmodel assigned to this view.
            ViewModelChanged.ObserveNested(vm => vm.IsVisibleChanged)
                .Subscribe(isVisible => { this.IsVisible = isVisible; });

            StackPanel stackPanel = new StackPanel
            {
                StackOrientation = Orientation.Horizontal
            };
            Children.Add(stackPanel);

            // Create a list of party views based on the party viewmodel.
            IReadOnlyReactiveList<SequenceDiagramColumnView> columnViews = ViewModelChanged
                .Where(vm => vm != null)
                .Select(vm => vm.PartyViewModels)
                .CreateDerivedListBinding(partyVM => new SequenceDiagramColumnView(this, partyVM))
                .ResultList;

            // Automatically add and remove party views to Children.
            columnViews.OnAdd.Subscribe(e => stackPanel.Children.Insert(e.Index, e.Element));
            columnViews.OnDelete.Subscribe(e => stackPanel.Children.RemoveAt(e.Index));
        }

        /// <see cref="OnMouseEvent"/>
        protected override bool OnMouseEvent(MouseEventData e)
        {
            // Add a new party on double click
            if (e.Id == MouseEvent.MOUSE_CLICKED && e.ClickCount % 2 == 0)
            {
                //Add a new Party.
                ViewModel.AddParty(e.MousePosition);
                return true;
            }
            else
            {
                return base.OnMouseEvent(e);
            }
        }

        /// <see cref="OnKeyEvent"/>
        protected override bool OnKeyEvent(KeyEventData eventData)
        {
            if (eventData.Id == KeyEvent.KEY_RELEASED &&
                eventData.KeyCode == 46 &&
                FocusedElement.GetType() == typeof(LabelView)
            )
            {
                // Delete party.
                PartyView partyView = (PartyView) FocusedElement.Parent;
                ViewModel.DeleteParty(partyView.ViewModel.Party);

                return true;
            }

            return false;
        }
    }


    /// <summary>
    /// The view for the sequence diagram column view.
    /// </summary>
    class SequenceDiagramColumnView : AnchorPanel
    {
        private readonly PartyView _partyView;
        private readonly LifeLineView _lifeLineView;

        public SequenceDiagramColumnView(SequenceDiagramView parent, PartyViewModel partyVM)
        {
            // Create the party view and add it to this column view.
            _partyView = new PartyView
            {
                ViewModel = partyVM
            };

            AnchorsProperty.SetValue(_partyView, Anchors.Left | Anchors.Top | Anchors.Right);
            Children.Add(_partyView);

            // Create the lifeline view and add it to this column view.
            _lifeLineView = new LifeLineView();
            MarginsProperty.SetValue(_lifeLineView, new Margins(0, _partyView.PreferredHeight, 0, 0));
            Children.Add(_lifeLineView);

            // Setup subscriptions.
            parent.ViewModelChanged.Select(vm => vm.StackVM)
                .Subscribe(stackVM => _lifeLineView.ViewModel = stackVM.CreateLifeLineForParty(partyVM));
        }
    }
}