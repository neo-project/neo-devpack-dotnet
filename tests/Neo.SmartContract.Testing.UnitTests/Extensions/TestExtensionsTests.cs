using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Extensions;
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

            var ret = (NeoToken.Candidate)stackItem.ConvertTo(typeof(NeoToken.Candidate));

            Assert.AreEqual(point, ret.PublicKey);
            Assert.AreEqual(1, ret.Votes);
        }
    }
}
