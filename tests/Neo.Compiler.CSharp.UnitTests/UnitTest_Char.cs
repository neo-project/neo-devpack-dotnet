using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Char : DebugAndTestBase<Contract_Char>
    {
        public static IEnumerable<object[]> CharTestData =>
            new List<object[]>
            {
                new object[] { '0', true, false, false, false, false },
                new object[] { '9', true, false, false, false, false },
                new object[] { 'a', false, true, false, true, false },
                new object[] { 'Z', false, true, false, false, true },
                new object[] { ' ', false, false, true, false, false },
                new object[] { '\t', false, false, true, false, false },
                new object[] { '$', false, false, false, false, false },
                new object[] { '\n', false, false, true, false, false },
            };

        [TestMethod]
        [DynamicData(nameof(CharTestData))]
        public void TestCharProperties(char c, bool isDigit, bool isLetter, bool isWhiteSpace, bool isLower, bool isUpper)
        {
            Assert.AreEqual(isDigit, Contract.TestCharIsDigit(c), $"IsDigit failed for '{c}'");
            Assert.AreEqual(1047330, Engine.FeeConsumed.Value);
            Assert.AreEqual(isLetter, Contract.TestCharIsLetter(c), $"IsLetter failed for '{c}'");
            Assert.AreEqual(1047990, Engine.FeeConsumed.Value);
            Assert.AreEqual(isWhiteSpace, Contract.TestCharIsWhiteSpace(c), $"IsWhiteSpace failed for '{c}'");
            Assert.AreEqual(isLower, Contract.TestCharIsLower(c), $"IsLower failed for '{c}'");
            Assert.AreEqual(1047330, Engine.FeeConsumed.Value);
            Assert.AreEqual(isUpper, Contract.TestCharIsUpper(c), $"IsUpper failed for '{c}'");
            Assert.AreEqual(1047330, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCharGetNumericValue()
        {
            Assert.AreEqual(0, Contract.TestCharGetNumericValue('0'));
            Assert.AreEqual(1047720, Engine.FeeConsumed.Value);
            Assert.AreEqual(9, Contract.TestCharGetNumericValue('9'));
            Assert.AreEqual(1047720, Engine.FeeConsumed.Value);
            Assert.AreEqual(-1, Contract.TestCharGetNumericValue('a'));
            Assert.AreEqual(1047600, Engine.FeeConsumed.Value);
            Assert.AreEqual(-1, Contract.TestCharGetNumericValue('$'));
            Assert.AreEqual(1047600, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCharSpecialCategories()
        {
            Assert.IsTrue(Contract.TestCharIsPunctuation('.'));
            Assert.AreEqual(1047450, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestCharIsPunctuation(','));
            Assert.AreEqual(1047450, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsPunctuation('a'));
            Assert.AreEqual(1048590, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestCharIsSymbol('$'));
            Assert.AreEqual(1047450, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestCharIsSymbol('+'));
            Assert.AreEqual(1047450, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsSymbol('a'));
            Assert.AreEqual(1049010, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestCharIsControl('\n'));
            Assert.AreEqual(1047990, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestCharIsControl('\0'));
            Assert.AreEqual(1047990, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsControl('a'));
            Assert.AreEqual(1047990, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCharSurrogates()
        {
            Assert.IsTrue(Contract.TestCharIsSurrogate('\uD800'));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestCharIsSurrogate('\uDFFF'));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsSurrogate('a'));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestCharIsHighSurrogate('\uD800'));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsHighSurrogate('\uDC00'));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsHighSurrogate('a'));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestCharIsLowSurrogate('\uDC00'));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsLowSurrogate('\uD800'));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsLowSurrogate('a'));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCharConversions()
        {
            Assert.AreEqual('A', Contract.TestCharToUpper('a'));
            Assert.AreEqual(1047990, Engine.FeeConsumed.Value);
            Assert.AreEqual('A', Contract.TestCharToUpper('A'));
            Assert.AreEqual(1047450, Engine.FeeConsumed.Value);
            Assert.AreEqual('a', Contract.TestCharToLower('A'));
            Assert.AreEqual(1047990, Engine.FeeConsumed.Value);
            Assert.AreEqual('a', Contract.TestCharToLower('a'));
            Assert.AreEqual(1047450, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCharIsLetterOrDigit()
        {
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('a'));
            Assert.AreEqual(1048170, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('A'));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('0'));
            Assert.AreEqual(1047450, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsLetterOrDigit('$'));
            Assert.AreEqual(1048170, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCharIsBetween()
        {
            Assert.IsTrue(Contract.TestCharIsBetween('a', 'a', 'z'));
            Assert.AreEqual(1047930, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestCharIsBetween('z', 'a', 'z'));
            Assert.AreEqual(1047930, Engine.FeeConsumed.Value);

            Assert.IsFalse(Contract.TestCharIsBetween('A', 'a', 'z'));
            Assert.AreEqual(1047990, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestCharIsBetween('0', 'a', 'z'));
            Assert.AreEqual(1047990, Engine.FeeConsumed.Value);
        }
    }
}
