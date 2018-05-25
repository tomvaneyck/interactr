using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using Interactr.Model;
using Interactr.Properties;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Dialogs;
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
            ViewModelChanged.ObserveNested(vm => vm.Label.TextChanged)
                .Where(newLabel => newLabel != null)
                .Subscribe(newLabel =>
                    {
                        if (!LabelView.IsInEditMode)
                        {
                            LabelView.Text = newLabel;
                        }
                    }
                );

            LabelView.TextChanged.Subscribe(newText =>
            {
                if (ViewModel != null) ViewModel.Label.Text = newText;
            });

            // Add child elements
            Children.Add(_actorImage);
            Children.Add(_objectRectangle);
            Children.Add(LabelView);

            // Bind CanApplyLabel and CanLeaveEditMode.
            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged)
                .Subscribe(canApplyLabel => LabelView.CanLeaveEditMode = canApplyLabel);

            // The label is red if CanApplyLabel is false.
            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged).Subscribe(canApplyLabel =>
            {
                LabelView.Color = canApplyLabel ? DefaultLabelColor : InvalidLabelColor;
            });

            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged)
                .Subscribe(canApplyLabel => _labelView.CanLeaveEditMode = canApplyLabel);

            // Fire ApplyLabel when leaving edit mode.
            LabelView.EditModeChanged.Subscribe(
                isInEditMode =>
                {
                    if (ViewModel != null)
                    {
                        ViewModel.LabelInEditMode = isInEditMode;

                        if (!isInEditMode)
                        {
                            ViewModel.ApplyLabel();
                        }
                    }
                }
            );

            // Bind text of label between this and PartyViewModel.
            LabelView.TextChanged.Subscribe(text =>
            {
                if (ViewModel != null)
                {
                    ViewModel.Label.Text = text;
                }
            });
        }

        /// <see cref="OnMouseEvent"/>
        protected override void OnMouseEvent(MouseEventData e)
        {
            if (e.Id == MouseEvent.MOUSE_CLICKED && e.ClickCount % 2 == 0 && FocusedElement.CanLoseFocus)
            {
                ViewModel.SwitchPartyType();
                Parent.Repaint();
                e.IsHandled = true;
            }
        }

        protected override void OnKeyEvent(KeyEventData e)
        {
            // On CTRL+Enter, open a party dialog
            if (e.Id == KeyEvent.KEY_PRESSED && Keyboard.IsKeyDown(KeyEvent.VK_CONTROL) && e.KeyCode == (int)Keys.Enter)
            {
                // Create dialog.
                var dialogVM = ViewModel.CreateNewDialogViewModel();
                var dialogView = new PartyDialogView {ViewModel = dialogVM};
                var window = Dialog.OpenDialog(this, dialogView, "Party settings", 230, 140);

                // Close dialog when the party is deleted or the close button is clicked.
                var windowsView = (WindowsView)window.Parent;
                ViewModel.Diagram.Parties.OnDelete
                    .Where(deleted => deleted.Element == ViewModel.Party)
                    .TakeUntil(window.WindowClosed)
                    .Subscribe(_ =>
                    {
                        windowsView.RemoveWindowWith(dialogView);
                    });

                e.IsHandled = true;
            }
        }
    }
}