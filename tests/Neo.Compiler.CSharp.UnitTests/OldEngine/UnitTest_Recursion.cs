using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.OldEngine
{
    [TestClass]
    public class UnitTest_Recursion
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Recursion.cs");
        }

        [TestMethod]
        public void Test_Factorial()
        {
            BigInteger prevResult = 1;
            BigInteger result;
            for (int i = 0; i < 10; i++)
            {
                testengine.Reset();
                result = testengine.ExecuteTestCaseStandard("factorial", i).Pop().GetInteger();
                Assert.AreEqual(i > 0 ? prevResult * i : 1, result);
                prevResult = result;
            }
        }

        [TestMethod]
        public void Test_HanoiTower()
        {
            int src = 100, aux = 200, dst = 300;
            Array result = testengine.ExecuteTestCaseStandard("hanoiTower", 1, src, aux, dst).Pop<Array>();
            Assert.AreEqual(result.Count, 1);
            List<(BigInteger rodId, BigInteger src, BigInteger dst)> expectedResult = new() { (1, src, dst) };
            for (int i = 0; i < expectedResult.Count; ++i)
            {
                StackItem[] step = ((Struct)result[i]).SubItems.ToArray();
                Assert.AreEqual(step[0], expectedResult[i].rodId);
                Assert.AreEqual(step[1], expectedResult[i].src);
                Assert.AreEqual(step[2], expectedResult[i].dst);
            }

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("hanoiTower", 3, src, aux, dst).Pop<Array>();
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
            Assert.IsTrue(testengine.ExecuteTestCaseStandard("odd", 7).Pop<Boolean>().GetBoolean());
            testengine.Reset();
            Assert.IsFalse(testengine.ExecuteTestCaseStandard("even", 9).Pop<Boolean>().GetBoolean());
            testengine.Reset();
            Assert.IsTrue(testengine.ExecuteTestCaseStandard("odd", -11).Pop<Boolean>().GetBoolean());
            testengine.Reset();
            Assert.IsTrue(testengine.ExecuteTestCaseStandard("even", -10).Pop<Boolean>().GetBoolean());
            testengine.Reset();
            Assert.IsFalse(testengine.ExecuteTestCaseStandard("even", -9).Pop<Boolean>().GetBoolean());
        }
    }
}
