using System;
using NUnit.Framework;
using Interactr.ViewModel;
using Interactr.Model;

namespace Interactr.Tests.ViewModel
{
    [TestFixture]
    public class PartyViewModelTests
    {
        [Test]
        public void SwitchPartyTypeTest1()
        {
            Party p = new Party(Party.PartyType.Actor, "t:T");
            PartyViewModel pvm = new PartyViewModel(p);
            pvm.SwitchPartyType();
            Assert.AreEqual(Party.PartyType.Object,p.Type);
        }

        [Test]
        public void SwitchPartyTypeTest2()
        {
            Party p = new Party(Party.PartyType.Object, "t:T");
            PartyViewModel pvm = new PartyViewModel(p);
            pvm.SwitchPartyType();
            Assert.AreEqual(Party.PartyType.Actor, p.Type);
        }
    }
}
