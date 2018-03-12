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
        public void BothUpperCase()
        {
            Assert.False(Party.IsValidLabel("A:B"));
        }
        [Test]
        public void BothLowerCase()
        {
            Assert.False(Party.IsValidLabel("a:b"));
        }
        [Test]
        public void InstanceUpperClassLower()
        {
            Assert.False(Party.IsValidLabel("A:b"));
        }
        [Test]
        public void InstanceLowerClassUpper()
        {
            Assert.True(Party.IsValidLabel("a:B"));
        }
        [Test]
        public void OnlyClassLower()
        {
            Assert.False(Party.IsValidLabel(":b"));
        }
        [Test]
        public void InstanceLowerClassUpperAndLower()
        {
            Assert.True(Party.IsValidLabel("a:Bc"));
        }
        [Test]
        public void BothUpperCaseClassLonger()
        {
            Assert.False(Party.IsValidLabel("A:BC"));
        }
        [Test]
        public void NoNames()
        {
            Assert.False(Party.IsValidLabel(":"));
        }
        [Test]
        public void OnlyInstanceLower()
        {
            Assert.False(Party.IsValidLabel("a:"));
        }
        [Test]
        public void OnlyInstanceUpper()
        {
            Assert.False(Party.IsValidLabel("A:"));
        }
        [Test]
        public void InstanceLowerLongerClassUpperAndLower()
        {
            Assert.True(Party.IsValidLabel("test:Test"));
        }
        [Test]
        public void BothUpperAndLower()
        {
            Assert.False(Party.IsValidLabel("Hey:Hey"));
        }
        [Test]
        public void BothLowerAndLonger()
        {
            Assert.False(Party.IsValidLabel("hi:hi"));
        }
        [Test]
        public void OnlyClassUpper()
        {
            Assert.True(Party.IsValidLabel(":B"));
        }
        [Test]
        public void NoColon()
        {
            Assert.False(Party.IsValidLabel("aaBb"));
            Assert.False(Party.IsValidLabel("AaBb"));
            Assert.False(Party.IsValidLabel("AAbB"));
            Assert.False(Party.IsValidLabel("AaBB"));
        }
        [Test]
        public void EmptyLabel()
        {
            Assert.False(Party.IsValidLabel(""));
        }
        [Test]
        public void NullLabel()
        {
            Assert.False(Party.IsValidLabel(null));
        }
    }
}
