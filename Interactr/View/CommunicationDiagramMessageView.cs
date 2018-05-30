using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Dialogs;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;
using Message = Interactr.Model.Message;
using Point = Interactr.View.Framework.Point;

namespace Interactr.View
{
    /// <summary>
    /// A view for a communication diagram message.
    /// </summary>
    public class CommunicationDiagramMessageView : UIElement
    {
        private static readonly Color DefaultLabelColor = Color.Black;
        private static readonly Color InvalidLabelColor = Color.Red;

        public Diagram Diagram { get;  }

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

        public CommunicationDiagramMessageView(Diagram diagram, MessageViewModel viewModel)
        {
            IsVisibleToMouse = false;

            Diagram = diagram;
            ViewModel = viewModel;

            Children.Add(_arrow);
            Children.Add(LabelWithMessageNumberView);

            // Change the size of the arrow views.
            WidthChanged.Subscribe(newWidth => _arrow.Width = newWidth);
            HeightChanged.Subscribe(newHeight => _arrow.Height = newHeight);

            // Update the label on a change.
            ViewModel.FormatString.TextChanged.Subscribe(_ =>
                LabelWithMessageNumberView.LabelView.Text = ViewModel.FormatString.Text);

            LabelWithMessageNumberView.LabelView.TextChanged.Subscribe(text =>
            {
                if (ViewModel != null)
                {
                    ViewModel.FormatString.Text = text;
                }
            });

            // Update the messageNumber on a change
            ViewModel.MessageNumberChanged.Subscribe(m => { LabelWithMessageNumberView.SetMessageNumber(m); });

            // Bind CanApplyLabel and CanLeaveEditMode.
            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged)
                .Subscribe(canApplyLabel => LabelWithMessageNumberView.LabelView.CanLeaveEditMode = canApplyLabel);

            // The label is red if CanApplyLabel is true.
            ViewModelChanged.ObserveNested(vm => vm.CanApplyLabelChanged)
                .Subscribe(canApplyLabel => LabelWithMessageNumberView.LabelView.Color =
                    canApplyLabel ? DefaultLabelColor : InvalidLabelColor);

            // Fire ApplyLabel when leaving edit mode.
            LabelWithMessageNumberView.LabelView.EditModeChanged.Subscribe(
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

            // Change the height and width of the LabelWithMessageNumberView manually because, 
            // the CommunicationDiagramMessageView is a UIElement.
            LabelWithMessageNumberView.PreferredHeightChanged.Subscribe(h => LabelWithMessageNumberView.Height = h);
            LabelWithMessageNumberView.PreferredWidthChanged.Subscribe(w => LabelWithMessageNumberView.Width = w);

            // Put the label under the arrow.
            Observable.CombineLatest(
                _arrow.StartPointChanged,
                _arrow.EndPointChanged
            ).Subscribe(p =>
            {
                // Set the labelMessageNumberView position
                LabelWithMessageNumberView.Position = _arrow.CalculateMidPoint();
            });
        }

        protected override void OnKeyEvent(KeyEventData e)
        {
            if (e.Id == KeyEvent.KEY_PRESSED && Keyboard.IsKeyDown(KeyEvent.VK_CONTROL) &&
                e.KeyCode == (int) Keys.Enter)
            {
                var windowsView = WalkToRoot().OfType<WindowsView>().FirstOrDefault();
                if (windowsView != null)
                {
                    if (ViewModel.MessageType == Message.MessageType.Invocation)
                    {
                        {
                            Debug.Print("Create dialog.");
                            // Create dialog.
                            var returnFormatStringVM = ViewModel.FormatString as InvocationFormatStringViewModel;
                            var dialogVM = returnFormatStringVM.CreateNewDialogViewModel(ViewModel.Message);
                            var dialogView = new InvocationMessageDialogView(dialogVM);
                            var window = Dialog.OpenDialog(this, dialogView, "Return Message settings", 350, 140);

                            ViewModel.Diagram.Messages.OnDelete.Where(deleted => deleted.Element == ViewModel.Message)
                                .TakeUntil(window.WindowClosed)
                                .Subscribe(_ => windowsView.RemoveWindowWith(dialogView));
                        }
                    }
                }

                e.IsHandled = true;
            }
        }
    }
}