using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Recursion : TestBase<Contract_Recursion>
    {
        public UnitTest_Recursion() : base(Contract_Recursion.Nef, Contract_Recursion.Manifest) { }

        [TestMethod]
        public void Test_Factorial()
        {
            BigInteger prevResult = 1;
            BigInteger result;
            for (int i = 0; i < 10; i++)
            {
                result = Contract.Factorial(i)!.Value;
                Assert.AreEqual(i > 0 ? prevResult * i : 1, result);
                prevResult = result;
            }
        }

        [TestMethod]
        public void Test_HanoiTower()
        {
            int src = 100, aux = 200, dst = 300;
            var result = Contract.HanoiTower(1, src, aux, dst)!;
            Assert.AreEqual(2788440, Engine.FeeConsumed.Value);
            Assert.AreEqual(result.Count, 1);
            List<(BigInteger rodId, BigInteger src, BigInteger dst)> expectedResult = [(1, src, dst)];
            for (int i = 0; i < expectedResult.Count; ++i)
            {
                StackItem[] step = ((Struct)result[i]).SubItems.ToArray();
                Assert.AreEqual(step[0], expectedResult[i].rodId);
                Assert.AreEqual(step[1], expectedResult[i].src);
                Assert.AreEqual(step[2], expectedResult[i].dst);
            }

            result = Contract.HanoiTower(3, src, aux, dst)!;
            expectedResult = new() {
                (1, src, dst),
                (2, src, aux),
                (1, dst, aux),
                (3, src, dst),
                (1, aux, src),
                (2, aux, dst),
                (1, src, dst),
            };
            for (int i = 0; i < expectedResult.Count; ++i)
            {
                StackItem[] step = ((Struct)result[i]).SubItems.ToArray();
                Assert.AreEqual(step[0], expectedResult[i].rodId);
                Assert.AreEqual(step[1], expectedResult[i].src);
                Assert.AreEqual(step[2], expectedResult[i].dst);
            }
        }

        [TestMethod]
        public void Test_MutualRecursion()
        {
            Assert.IsTrue(Contract.Odd(7));
            Assert.AreEqual(1181940, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.Even(9));
            Assert.AreEqual(1220160, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.Odd(-11));
            Assert.AreEqual(1259040, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.Even(-10));
            Assert.AreEqual(1239870, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.Even(-9));
            Assert.AreEqual(1220700, Engine.FeeConsumed.Value);
        }
    }
}
