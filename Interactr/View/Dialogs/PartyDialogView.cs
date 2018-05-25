using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Model;
using Interactr.Reactive;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Interactr.ViewModel.Dialogs;

namespace Interactr.View.Dialogs
{
    public class PartyDialogView : AnchorPanel
    {
        #region ViewModel

        private readonly ReactiveProperty<PartyDialogViewModel> _viewModel = new ReactiveProperty<PartyDialogViewModel>();

        public PartyDialogViewModel ViewModel
        {
            get => _viewModel.Value;
            set => _viewModel.Value = value;
        }

        public IObservable<PartyDialogViewModel> ViewModelChanged => _viewModel.Changed;

        #endregion

        public PartyDialogView()
        {
            LabelView instanceNameLabel = new LabelView
            {
                Text = "Instance name:",
                IsVisibleToMouse = false,
                IsReadOnly = true
            };
            AnchorsProperty.SetValue(instanceNameLabel, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(instanceNameLabel, new Margins(5, 5));
            this.Children.Add(instanceNameLabel);

            TextBox instanceNameTextBox = new TextBox();
            instanceNameTextBox.TextChanged.Subscribe(newText => ViewModel.InstanceName = newText);
            ViewModelChanged.ObserveNested(vm => vm.InstanceNameChanged).Subscribe(newText => instanceNameTextBox.Text = newText);
            AnchorsProperty.SetValue(instanceNameTextBox, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(instanceNameTextBox, new Margins(115, 5));
            this.Children.Add(instanceNameTextBox);

            LabelView classNameLabel = new LabelView
            {
                Text = "Class name:",
                IsVisibleToMouse = false,
                IsReadOnly = true
            };
            AnchorsProperty.SetValue(classNameLabel, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(classNameLabel, new Margins(5, 30));
            this.Children.Add(classNameLabel);

            TextBox classNameTextBox = new TextBox();
            classNameTextBox.TextChanged.Subscribe(newText => ViewModel.ClassName = newText);
            ViewModelChanged.ObserveNested(vm => vm.ClassNameChanged).Subscribe(newText => classNameTextBox.Text = newText);
            AnchorsProperty.SetValue(classNameTextBox, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(classNameTextBox, new Margins(115, 30));
            this.Children.Add(classNameTextBox);

            LabelView partyTypeLabel = new LabelView
            {
                Text = "Party type:",
                IsVisibleToMouse = false,
                IsReadOnly = true
            };
            AnchorsProperty.SetValue(partyTypeLabel, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(partyTypeLabel, new Margins(5, 55));
            this.Children.Add(partyTypeLabel);

            RadioButtonGroup.RadioButton actorButton = new RadioButtonGroup.RadioButton
            {
                Label = "Actor"
            };
            actorButton.IsSelectedChanged.Where(selected => selected).Subscribe(_ => ViewModel.PartyType = Party.PartyType.Actor);
            ViewModelChanged.ObserveNested(vm => vm.PartyTypeChanged).Where(type => type == Party.PartyType.Actor).Subscribe(_ => actorButton.IsSelected = true);

            RadioButtonGroup.RadioButton objectButton = new RadioButtonGroup.RadioButton
            {
                Label = "Object"
            };
            objectButton.IsSelectedChanged.Where(selected => selected).Subscribe(_ => ViewModel.PartyType = Party.PartyType.Object);
            ViewModelChanged.ObserveNested(vm => vm.PartyTypeChanged).Where(type => type == Party.PartyType.Object).Subscribe(_ => objectButton.IsSelected = true);

            RadioButtonGroup partyTypeRadioGroup = new RadioButtonGroup
            {
                Children =
                {
                    actorButton,
                    objectButton
                }
            };
            AnchorsProperty.SetValue(partyTypeRadioGroup, Anchors.Left | Anchors.Top);
            MarginsProperty.SetValue(partyTypeRadioGroup, new Margins(115, 55));

            this.Children.Add(partyTypeRadioGroup);
        }
    }
}
