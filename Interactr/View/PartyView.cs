using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using Interactr.Model;
using Interactr.Properties;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
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
        private static readonly Color DefaultLabelColor = Color.Black;
        private static readonly Color InvalidLabelColor = Color.Red;

        #region ViewModel

        private readonly ReactiveProperty<PartyViewModel> _viewModel = new ReactiveProperty<PartyViewModel>();

        public PartyViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<PartyViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        protected readonly ImageView _actorImage = new ImageView();
        protected readonly RectangleView _objectRectangle = new RectangleView();
        protected readonly LabelView _labelView = new LabelView();

        /// <summary>
        /// The label view associated with this party view.
        /// </summary>
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
            LabelView.Position = new Point(0, 125);
            AnchorsProperty.SetValue(LabelView, Anchors.Bottom);

            // Define the display to be the view that matches the party type
            ViewModelChanged.ObserveNested(vm => vm.TypeChanged).Subscribe(partyType =>
            {
                Debug.WriteLine("Switch type");
                _actorImage.IsVisible = partyType == Party.PartyType.Actor;
                _objectRectangle.IsVisible = partyType == Party.PartyType.Object;
            });

            // Bi-directional bind party label to view
            ViewModelChanged.ObserveNested(vm => vm.LabelChanged)
                .Subscribe(newLabel => LabelView.Text = newLabel);
            LabelView.TextChanged.Subscribe(newText =>
            {
                if (ViewModel != null) ViewModel.Label = newText;
            });

            // On position change in the viewmodel change the position in the view.
            ViewModelChanged.ObserveNested(vm => vm.PositionChanged)
                .Subscribe(newPosition => this.Position = newPosition);

            // Add child elements
            Children.Add(_actorImage);
            Children.Add(_objectRectangle);
            Children.Add(LabelView);

            // Bind CanApplyLabel and CanLeaveEditMode.
            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged)
                .Subscribe(canApplyLabel => LabelView.CanLeaveEditMode = canApplyLabel);

            // The label is red if CanApplyLabel is true.
            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged)
                .Subscribe(canApplyLabel => LabelView.Color = canApplyLabel ? DefaultLabelColor : InvalidLabelColor);

            // Bind text of label between this and PartyViewModel.
            _labelView.TextChanged.Subscribe(text =>
            {
                if (ViewModel != null)
                {
                    ViewModel.Label = text;
                }
            });

            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged)
                .Subscribe(canApplyLabel => _labelView.CanLeaveEditMode = canApplyLabel);

            // Fire ApplyLabel when leaving edit mode.
            LabelView.EditModeChanged.Subscribe(
                isInEditMode =>
                {
                    if (ViewModel != null && !isInEditMode) ViewModel.ApplyLabel();
                }
            );

            // Bind text of label between this and PartyViewModel.
            LabelView.TextChanged.Subscribe(text =>
            {
                if (ViewModel != null)
                {
                    ViewModel.Label = text;
                }
            });
        }

        /// <see cref="OnMouseEvent"/>
        protected override bool OnMouseEvent(MouseEventData e)
        {
            if (e.Id == MouseEvent.MOUSE_CLICKED && e.ClickCount % 2 == 0 && FocusedElement.CanLoseFocus)
            {
                Debug.WriteLine("Click registered.");
                ViewModel.SwitchPartyType();
                Parent.Repaint();
                return true;
            }

            return false;
        }
    }
}