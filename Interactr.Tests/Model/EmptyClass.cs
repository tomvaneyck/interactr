using System;
using Interactr.Model;
using NUnit.Framework;
using System.Collections.Generic;
namespace Interactr.Tests.Model
{
    [TestFixture]
    public class LabelTests
    {
        [Test]
        public void Test_1()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel("A;B"));
           
        }
        [Test]
        public void Test_2()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel("a;b"));

        }
        [Test]
        public void Test_3()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel("A;b"));

        }
        [Test]
        public void Test_4()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(true, p.IsValidLabel("a;B"));

        }
        [Test]
        public void Test_5()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel(";b"));

        }
        [Test]
        public void Test_6()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(true, p.IsValidLabel("a;Bc"));
        }
        [Test]
        public void Test_7()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel("A;BC"));
        }
        [Test]
        public void Test_8()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel(";"));
        }
        [Test]
        public void Test_9()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel("a;"));
        }
        [Test]
        public void Test_10()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel("A;"));
        }
        [Test]
        public void Test_11()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(true, p.IsValidLabel("test;Test"));
        }
        [Test]
        public void Test_12()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel("Hey;Hey"));
        }
        [Test]
        public void Test_13()
        {
            Party p = new Party(Party.PartyType.Object, "a");
            Assert.AreEqual(false, p.IsValidLabel("hi;hi"));
        }
    }
}
