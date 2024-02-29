using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.Native.Models;
using Neo.VM;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.SmartContract.TestEngine.UnitTests.Extensions
{
    [TestClass]
    public class TestExtensionsTests
    {
        [TestMethod]
        public void TestConvertEnum()
        {
            StackItem stackItem = new Integer((int)VMState.FAULT);

            Assert.AreEqual(VMState.FAULT, (VMState)stackItem.ConvertTo(typeof(VMState)));
        }

        [TestMethod]
        public void TestClass()
        {
            var point = ECCurve.Secp256r1.G;
            StackItem stackItem = new Array(new StackItem[] { point.ToArray(), BigInteger.One });

            var ret = (Candidate)stackItem.ConvertTo(typeof(Candidate));

            Assert.AreEqual(point, ret.PublicKey);
            Assert.AreEqual(1, ret.Votes);
        }

        [TestMethod]
        public void TestValueType()
        {
            StackItem stackItem = new Array(new StackItem[] { 1, 2 });
            var ret = stackItem.ConvertTo(typeof((int, int)));

            Assert.IsTrue(ret.GetType().IsValueType);
        }
    }
}
