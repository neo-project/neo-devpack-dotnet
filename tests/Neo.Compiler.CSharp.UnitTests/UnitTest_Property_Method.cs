using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System.Numerics;
using Akka.Util.Internal;
using Array = Neo.VM.Types.Array;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property_Method : DebugAndTestBase<Contract_PropertyMethod>
    {
        [TestMethod]
        public void TestPropertyMethod()
        {
            var arr = Contract.TestProperty()!;
            AssertGasConsumed(2053500);

            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual((arr[0] as StackItem)!.GetString(), "NEO3");
            Assert.AreEqual(arr[1], new BigInteger(10));
        }

        [TestMethod]
        public void TestPropertyMethod2()
        {
            Contract.TestProperty2();
            AssertGasConsumed(1557360);
            // No errors
        }

        [TestMethod]
        public void TestPropertyMethod3()
        {
            var person = Contract.TestProperty3()! as Array;
            AssertGasConsumed(1554990); // Adjust this value based on actual gas consumption

            Assert.IsNotNull(person);
            Assert.AreEqual(2, person.Count);
            Assert.AreEqual((person[0] as StackItem)!.GetString(), "NEO3");
            Assert.AreEqual(person[1], new BigInteger(10));
        }

        [TestMethod]
        public void TestPropertyMethod4()
        {
            var map = Contract.TestProperty4()!;
            AssertGasConsumed(1230570); // Adjust this value based on actual gas consumption

            Assert.IsNotNull(map);
            Assert.AreEqual(1, map.Count);

            map.Keys.ForEach(Console.WriteLine);

            var key = (ByteString)"Name";
            Assert.IsTrue(map.ContainsKey(key));
            Assert.AreEqual((map[key] as StackItem)!.GetString(), "NEO3");
        }

        [TestMethod]
        public void TestPropertyMethod5()
        {
            var list = Contract.TestProperty5()!;
            AssertGasConsumed(1046190); // Adjust this value based on actual gas consumption

            Assert.IsNotNull(list);
            Assert.AreEqual(5, list.Count);
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(i + 1, (int)(BigInteger)list[i]);
            }
        }
    }
}
