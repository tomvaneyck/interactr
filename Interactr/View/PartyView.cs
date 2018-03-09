using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Properties;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.ViewModel;
using Interactr.Window;
using Point = Interactr.View.Framework.Point;

namespace Interactr.View
{
    /// <summary>
    /// A view for one party.
    /// </summary>
    public class PartyView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<PartyViewModel> _viewModel = new ReactiveProperty<PartyViewModel>();

        public PartyViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<PartyViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        private readonly ImageView _actorImage = new ImageView();
        private readonly RectangleView _objectRectangle = new RectangleView();
        private readonly LabelView _labelView = new LabelView();

        public LabelView LabelView
        {
            get => _labelView;
        }

        public PartyView()
        {
            // Set the image
            _actorImage.Image = Resources.StickFigure;
            _actorImage.PreferredWidth = 125;
            _actorImage.PreferredHeight = 125;

            // Set layout
            MarginsProperty.SetValue(_actorImage, new Margins(0, 0, 0, 25));
            MarginsProperty.SetValue(_objectRectangle, new Margins(0, 0, 0, 25));
            _labelView.Position = new Point(0, 125);
            AnchorsProperty.SetValue(_labelView, Anchors.Bottom);

            // Define the display to be the view that matches the party type
            ViewModelChanged.ObserveNested(vm => vm.TypeChanged).Subscribe(partyType =>
            {
                _actorImage.IsVisible = partyType == Party.PartyType.Actor;
                _objectRectangle.IsVisible = partyType == Party.PartyType.Object;
            });

            // Bi-directional bind party label to view
            ViewModelChanged.ObserveNested(vm => vm.LabelChanged)
                .Subscribe(newLabel => _labelView.Text = newLabel);
            _labelView.TextChanged.Subscribe(newText =>
            {
                if (ViewModel != null) ViewModel.Label = newText;
            });

            // On double click, change party type
            _objectRectangle.MouseEventOccured.Merge(_actorImage.MouseEventOccured)
                .Where(e => e.Id == MouseEvent.MOUSE_CLICKED &&
                            e.ClickCount % 2 == 0) // Modulo for consequent double clicks.
                .Subscribe(_ => ViewModel?.SwitchPartyType());

            // On position change in the viewmodel change the position in the view.
            ViewModelChanged.ObserveNested(vm => vm.PositionChanged)
                .Subscribe(newPosition => this.Position = newPosition);

            // Add child elements
            Children.Add(_actorImage);
            Children.Add(_objectRectangle);
            Children.Add(_labelView);

            // Bind CanApplyLabel and CanLeaveEditMode.
            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged)
                .Subscribe(canApplyLabel => _labelView.CanLeaveEditMode = canApplyLabel);

            // Fire ApplyLabel when leaving edit mode.
            _labelView.EditModeChanged.Subscribe(
                isInEditMode =>
                {
                    if (ViewModel != null && !isInEditMode) ViewModel.ApplyLabel();
                }
            );
        }
    }
}