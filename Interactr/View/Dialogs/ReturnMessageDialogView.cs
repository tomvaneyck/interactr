﻿using System;
using System.Reactive.Linq;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.ViewModel.Dialogs;

namespace Interactr.View.Dialogs
{
    public class ReturnMessageDialogView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<ReturnMessageDialogViewModel> _viewModel =
            new ReactiveProperty<ReturnMessageDialogViewModel>();

        public ReturnMessageDialogViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<ReturnMessageDialogViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public ReturnMessageDialogView(ReturnMessageDialogViewModel viewModel)
        {
            ViewModel = viewModel;
            IsTabScope = true;

            // Add a label
            LabelView returnMessageLabel = new LabelView
            {
                Text = "Label:",
                IsVisibleToMouse = false,
                IsReadOnly = true
            };
            AnchorsProperty.SetValue(returnMessageLabel, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(returnMessageLabel, new Margins(5, 5));
            this.Children.Add(returnMessageLabel);

            // Add a textbox for typing the messageLabel.
            TextBox returnMessageTextBox = new TextBox();

            // Bind TextBox and viewmodel.
            ViewModel.TextChanged.Subscribe(newText => returnMessageTextBox.Text = newText);
            returnMessageTextBox.TextChanged.Subscribe(newText => ViewModel.Text = newText);

            // Anchor and margin properties.
            AnchorsProperty.SetValue(returnMessageTextBox, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(returnMessageTextBox, new Margins(60, 5));

            this.Children.Add(returnMessageTextBox);
        }
    }
}