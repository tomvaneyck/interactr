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
            Diagram diagram = new Diagram();
            Party p = new Party(Party.PartyType.Actor, "t:T");
            diagram.Parties.Add(p);

            PartyViewModel pvm = new PartyViewModel(diagram, p);
            pvm.SwitchPartyType();
            Assert.AreEqual(Party.PartyType.Object,p.Type);
        }

        [Test]
        public void SwitchPartyTypeTest2()
        {
            Diagram diagram = new Diagram();
            Party p = new Party(Party.PartyType.Object, "t:T");
            diagram.Parties.Add(p);

            PartyViewModel pvm = new PartyViewModel(diagram, p);
            pvm.SwitchPartyType();
            Assert.AreEqual(Party.PartyType.Actor, p.Type);
        }
    }
}
