using Interactr.Model;
using Interactr.View;
using Interactr.View.Controls;
using Interactr.ViewModel;
using NUnit.Framework;
using System;

namespace Interactr.Tests.View
{
    [TestFixture]
    [Category("RequiresUI")]
    public class PartyViewTests
    {
        // Exposing _labelView by inheritance.
        private class PartyViewTest : PartyView
        {
            public LabelView LabelView => _labelView;
        }

        // Party variable.
        private PartyViewTest partyView;

        [SetUp]
        public void SetupPartyViewTest()
        {
            partyView = new PartyViewTest
            {
                ViewModel = new PartyViewModel(new Party(Party.PartyType.Actor, "instance:Classname"))
            };
        }

        [Test]
        public void CanLeaveEditAndCanApplyLabelFalseTest()
        {
            
            partyView.LabelView.IsInEditMode = true;
            partyView.LabelView.Text = "test:isWrong";

            Assert.False(partyView.LabelView.CanLeaveEditMode);
            Assert.False(partyView.ViewModel.CanApplyLabel);

            Assert.Throws(typeof(ArgumentException), () => partyView.LabelView.IsInEditMode = false);
        }

        [Test]
        public void CanLeaveEditAndCanApplyLabelTrueTest()
        {
            partyView.LabelView.IsInEditMode = true;
            partyView.LabelView.Text = "test:IsRight";

            Assert.True(partyView.LabelView.CanLeaveEditMode);
            Assert.True(partyView.ViewModel.CanApplyLabel);
        }

        [Test]
        public void CanLeaveEditAndCanApplyLabelChangeTest()
        {
            partyView.LabelView.IsInEditMode = true;
            partyView.LabelView.Text = "test:IsRight";

            partyView.LabelView.IsInEditMode = false;

            Assert.False(partyView.LabelView.IsInEditMode);

            partyView.LabelView.IsInEditMode = true;
            partyView.LabelView.Text = "test:isWrong";

            Assert.False(partyView.LabelView.CanLeaveEditMode);
            Assert.False(partyView.ViewModel.CanApplyLabel);

            Assert.Throws(typeof(ArgumentException), () => partyView.LabelView.IsInEditMode = false);
        }

        [Test]
        public void LabelTextBindingPartyViewModelAndPartyViewTest()
        {
            partyView.LabelView.Text = "test:IsRight";

            Assert.AreEqual(partyView.ViewModel.Label, partyView.LabelView.Text);
        }

        [Test]
        public void OnLeaveEditModeApplyLabelTest()
        {
            partyView.LabelView.IsInEditMode = true;
            partyView.LabelView.Text = "test:IsRight";
            partyView.LabelView.IsInEditMode = false;

            Assert.AreEqual(partyView.ViewModel.Label, partyView.ViewModel.Party.Label);
        }
    }
}
