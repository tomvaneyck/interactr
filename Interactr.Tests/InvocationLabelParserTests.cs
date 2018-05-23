using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Interactr.Tests
{
    [TestFixture]
    public class InvocationLabelParserTests
    {
        // Valid Test labels
        private const string ValidMessageLabelNormal = "testlabel()";
        private const string ValidMessageLabelWithNumber = "testlabel2()";
        private const string ValidMessageLabelWithCapitals = "testLABEL(arg3)";
        private const string ValidMessageLabelWithArguments = "testlabel(arg1,arg2,arg3)";
        private const string ValidMessageLabelWithCapitalInArguments = "testlabel(Arg1,ARG2,aRG3)";
        private const string ValidMessageLabelUnderscores = "test_label()";

        [Test]
        public void RetrieveMethodNameValidMessageLabelTest()
        {
            string expected = "testlabel";
            string actual = InvocationLabelParser.RetrieveMethodNameFromLabel(ValidMessageLabelNormal);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RetrieveMethodNameLabelWithNumberTest()
        {
            string expected = "testlabel2";
            string actual = InvocationLabelParser.RetrieveMethodNameFromLabel(ValidMessageLabelWithNumber);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RetrieveMethodNameLabelWithCapitalsTest()
        {
            string expected = "testLABEL";
            string actual = InvocationLabelParser.RetrieveMethodNameFromLabel(ValidMessageLabelWithCapitals);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RetrieveMethodNameLabelWithArgumentsTest()
        {
            string expected1 = "testlabel";
            string actual1 = InvocationLabelParser.RetrieveMethodNameFromLabel(ValidMessageLabelWithArguments);

            string expected2 = "testlabel";
            string actual2 = InvocationLabelParser.RetrieveMethodNameFromLabel(ValidMessageLabelWithCapitalInArguments);

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
        }

        [Test]
        public void RetrieveMethodNameLabelWithUnderscoreTest()
        {
            string expected = "test_label";
            string actual = InvocationLabelParser.RetrieveMethodNameFromLabel(ValidMessageLabelUnderscores);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RetrieveArgumentsEmpty()
        {
            List<string> expected1 = new List<string>() {""};
            var actual1 = 
                InvocationLabelParser.RetrieveArgumentsFromLabel(ValidMessageLabelNormal);

            var expected2 = new List<string>() {""};
            var actual2 =
                InvocationLabelParser.RetrieveArgumentsFromLabel(ValidMessageLabelWithNumber);

            var expected3 = new List<string>() {""};
            var actual3 = InvocationLabelParser.RetrieveArgumentsFromLabel(ValidMessageLabelUnderscores);

            CollectionAssert.AreEqual(expected1, actual1);
            CollectionAssert.AreEqual(expected2, actual2);
            CollectionAssert.AreEqual(expected3, actual3);
        }

        [Test]
        public void RetrieveSingleArgument()
        {
            List<string> expected = new List<string>() {"arg3"};
            var actual = InvocationLabelParser.RetrieveArgumentsFromLabel(ValidMessageLabelWithCapitals);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void RetrieveMultipleArgumentsLowerCase()
        {
            List<string> expected = new List<string>() {"arg1", "arg2", "arg3"};
            var actual = InvocationLabelParser.RetrieveArgumentsFromLabel(ValidMessageLabelWithArguments);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void RetrieveMultipleArgumentsWithUpperCase()
        {
            List<string> expected = new List<string>() {"Arg1", "ARG2", "aRG3"};
            var actual = InvocationLabelParser.RetrieveArgumentsFromLabel(ValidMessageLabelWithCapitalInArguments);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}