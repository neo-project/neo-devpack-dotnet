using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class PointerTest : DebugAndTestBase<Contract_Pointers>
    {
        [TestMethod]
        public void Test_CreatePointer()
        {
            var item = Contract.CreateFuncPointer();
            Assert.IsInstanceOfType(item, typeof(Pointer));

            // Test pointer

            item = Engine.Execute(Contract_Pointers.Nef.Script, ((Pointer)item).Position, (e) => { e.Push(1); });

            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void Test_ExecutePointer()
        {
            Assert.AreEqual(123, Contract.CallFuncPointer());
        }

        [TestMethod]
        public void Test_ExecutePointerWithArgs()
        {
            // Internall

            Assert.AreEqual(new BigInteger(new byte[] { 11, 22, 33 }), Contract.CallFuncPointerWithArg());

            // With pointer

            var item = Contract.CreateFuncPointerWithArg();
            Assert.IsInstanceOfType(item, typeof(Pointer));

            // Test pointer

            item = Engine.Execute(Contract_Pointers.Nef.Script, ((Pointer)item).Position, (e) => { e.Push(123); });
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetInteger());
        }
    }
}
