using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Interactr.Model;
using NUnit.Framework;
using System.Reactive.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using static System.Object;

namespace Interactr.Tests.Model
{
    [TestFixture]
    public class PartyObservablesTest
    {
        //Test labels
        private const string validLabel1 = "instanceName;Classname";
        private const string validLabel2 = "instanceName2;Classname2";

        // Test parties
        private Party _defaultTestParty;
        private List<string> _labelResultCollector;
        private List<Party.PartyType> _partyTypeResultCollector;

        [OneTimeSetUp]
        public void init()
        {
            // initialize test party.
            _defaultTestParty = new Party(Party.PartyType.Actor, validLabel1);
            _labelResultCollector = new List<string>();
            _partyTypeResultCollector = new List<Party.PartyType>();
        }

        [SetUp]
        public void before()
        {
            _defaultTestParty = new Party(Party.PartyType.Actor, validLabel1);
            // Create  new resultcollectors before every test.
            _labelResultCollector.Clear();
            _partyTypeResultCollector.Clear();

            //Subscribe to observables.
            ObservableExtensions.Subscribe(_defaultTestParty.labelChanged, x => _labelResultCollector.Add(x));
            ObservableExtensions.Subscribe(_defaultTestParty.TypeChanged, x => _partyTypeResultCollector.Add(x));
        }

        [Test]
        public void NoLabelChangeObservableTest()
        {
            Assert.That(_labelResultCollector, Has.Count.EqualTo(1));
            Assert.That(_labelResultCollector, Has.Member(validLabel1));
        }

        [Test]
        public void AddOneLabelObservableTest()
        {
            _defaultTestParty.Label = validLabel2;

            // Assert that the change was recorded and the value is correct.
            //Assert that the resultcollector contains the initial label and new label.
            Assert.That(_labelResultCollector, Has.Count.EqualTo(2));
            Assert.That(_labelResultCollector, Has.Member(validLabel1));
            Assert.That(_labelResultCollector, Has.Member(validLabel2));
        }

        [Test]
        public void AddTwoLabelsObservableTest()
        {
            _defaultTestParty.Label = validLabel1;
            _defaultTestParty.Label = validLabel2;

            // Assert that resultcollector contains 3 labels: the initial label and the 2 changes. 
            Assert.That(_labelResultCollector, Has.Count.EqualTo(3));
            Assert.That(_labelResultCollector, Has.Member(validLabel1));
            Assert.That(_labelResultCollector, Has.Member(validLabel2));
        }

        [Test]
        public void NoTypeChangeObservableTest()
        {
            //Assert that the result collector only has the initial value.
            Assert.That(_partyTypeResultCollector, Has.Count.EqualTo(1));
            Assert.That(_partyTypeResultCollector, Has.Member(_defaultTestParty.Type));
        }

        [Test]
        public void ChangePartyTypeOnceObservableTest()
        {
            Party.PartyType oldPartyType = _defaultTestParty.Type;
            Party.PartyType newPartyType = Party.PartyType.Object;
            _defaultTestParty.Type = newPartyType;

            //Assert that the resultcollector contains the initial value and the new value.
            Assert.That(_partyTypeResultCollector, Has.Count.EqualTo(2));
            Assert.That(_partyTypeResultCollector, Has.Member(oldPartyType));
            Assert.That(_partyTypeResultCollector, Has.Member(newPartyType));
        }

        [Test]
        public void ChangePartyTypeTwiceObservablesTest()
        {
            _defaultTestParty.Type = Party.PartyType.Object;
            _defaultTestParty.Type = Party.PartyType.Actor;

            // Assert that resultcollector contains 3 labels. 
            Assert.That(_partyTypeResultCollector, Has.Count.EqualTo(3));
        }
    }
}