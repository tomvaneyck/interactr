﻿using Interactr.Model;
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
        // Party variable.
        private Diagram diagram;
        private Party party;
        private PartyView partyView;


        [SetUp]
        public void SetupPartyViewTest()
        {
            party = new Party(Party.PartyType.Actor, "instance:Classname");
            diagram = new Diagram {Parties = { party }};
            
            partyView = new PartyView
            {
                ViewModel = new PartyViewModel(diagram, party)
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

            Assert.AreEqual(partyView.ViewModel.Label.Text, partyView.LabelView.Text);
        }

        [Test]
        public void OnLeaveEditModeApplyLabelTest()
        {
            partyView.LabelView.IsInEditMode = true;
            partyView.LabelView.Text = "test:IsRight";
            partyView.LabelView.IsInEditMode = false;

            Assert.AreEqual(partyView.ViewModel.Label.Text, partyView.ViewModel.Party.Label);
        }
    }
}
