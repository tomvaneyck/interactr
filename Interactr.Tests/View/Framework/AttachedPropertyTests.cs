using Interactr.View.Framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Tests.View.Framework
{
    [TestFixture]
    public class AttachedPropertyTests
    {
        private AttachedProperty<string> _attachedProperty;
        private string _testString = "someText";
        private string _secondTestString = "someOtherText";
        private UIElement _uiElement = new UIElement();

        [SetUp]
        public void SetupTests()
        {
            _attachedProperty = new AttachedProperty<string>(_testString);
        }

        [Test]
        public void DefaultValueObjectTest()
        {
            Assert.AreEqual(_testString, _attachedProperty.DefaultValueObject);
        }

        [Test]
        public void DefaultValueTest()
        {
            Assert.AreEqual(_testString, _attachedProperty.DefaultValue);
        }

        [Test]
        public void GetValueObjectTest()
        {
            Assert.AreEqual(_testString, _attachedProperty.GetValueObject(_uiElement));

            _uiElement.AttachedProperties[_attachedProperty] = _secondTestString;
            Assert.AreEqual(_secondTestString, _attachedProperty.GetValueObject(_uiElement));
        }

        [Test]
        public void SetValueObjectTest()
        {
            Assert.Throws(typeof(ArgumentException), () =>
            {
                _attachedProperty.SetValueObject(_uiElement, 5);
            });

            _attachedProperty.SetValueObject(_uiElement, _secondTestString);
            Assert.AreEqual(_secondTestString, _uiElement.AttachedProperties[_attachedProperty]);
        }

        [Test]
        public void GetValueTest()
        {
            _uiElement.AttachedProperties[_attachedProperty] = _secondTestString;
            Assert.AreEqual(_secondTestString, _attachedProperty.GetValue(_uiElement));
        }

        [Test]
        public void SetValueTest()
        {
            _attachedProperty.SetValue(_uiElement, _secondTestString);
            Assert.AreEqual(_secondTestString, _uiElement.AttachedProperties[_attachedProperty]);
        }
    }
}
