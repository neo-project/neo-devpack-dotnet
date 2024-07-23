using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Returns : TestBase<Contract_Returns>
    {
        public UnitTest_Returns() : base(Contract_Returns.Nef, Contract_Returns.Manifest) { }

        [TestMethod]
        public void Test_OneReturn()
        {
            Assert.AreEqual(new BigInteger(-4), Contract.Subtract(5, 9));
            Assert.AreEqual(1002108400, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_DoubleReturnA()
        {
            var array = Contract.Div(9, 5)!;
            Assert.AreEqual(1002600580, Engine.FeeConsumed.Value);

            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(BigInteger.One, array[0]);
            Assert.AreEqual(new BigInteger(4), array[1]);
        }

        [TestMethod]
        public void Test_VoidReturn()
        {
            Assert.AreEqual(new BigInteger(14), Contract.Sum(9, 5));
            Assert.AreEqual(1002108400, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_DoubleReturnB()
        {
            Assert.AreEqual(new BigInteger(-3), Contract.Mix(9, 5));
            Assert.AreEqual(1002697900, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteStringAdd()
        {
            Assert.AreEqual("helloworld", Encoding.ASCII.GetString(Contract.ByteStringAdd(Encoding.ASCII.GetBytes("hello"), Encoding.ASCII.GetBytes("world"))!));
        }
    }
}
