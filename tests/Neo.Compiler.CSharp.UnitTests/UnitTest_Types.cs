using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types : TestBase<Contract_Types>
    {
        public UnitTest_Types() : base(Contract_Types.Nef, Contract_Types.Manifest) { }

        [TestMethod]
        public void Null_Test()
        {
            Assert.IsNull(Contract.CheckNull());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Bool_Test()
        {
            Assert.IsTrue(Contract.CheckBoolTrue());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.CheckBoolFalse());
            Assert.AreEqual(1003044040, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void ByteStringConcat_Test()
        {
            Assert.AreEqual("1212", Contract.ConcatByteString([(byte)'1'], [(byte)'2']));
            Assert.AreEqual(1003045120, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void ToAddress_Test()
        {
            Assert.AreEqual("NdtB8RXRmJ7Nhw1FPTm7E6HoDZGnDw37nf", Contract.ToAddress(UInt160.Parse("820944cfdc70976602d71b0091445eedbc661bc5"), 53));
            Assert.AreEqual(1005650920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void CheckEnumArg_Test()
        {
            var methods = Contract_Types.Manifest.Abi.Methods;
            var checkEnumArg = methods.Where(u => u.Name == "checkEnumArg").First();
            Assert.AreEqual(new JArray(checkEnumArg.Parameters.Select(u => u.ToJson()).ToArray()).ToString(false), @"[{""name"":""arg"",""type"":""Integer""}]");
        }

        [TestMethod]
        public void CheckBoolString_Test()
        {
            Assert.AreEqual(true.ToString(), Contract.CheckBoolString(true));
            Assert.AreEqual(1002123190, Engine.FeeConsumed.Value);
            Assert.AreEqual(false.ToString(), Contract.CheckBoolString(false));
            Assert.AreEqual(1003170640, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Sbyte_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckSbyte());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Byte_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckByte());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Short_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckShort());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Ushort_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckUshort());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Int_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckInt());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Uint_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckUint());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Long_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckLong());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Ulong_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckUlong());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void BigInteger_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckBigInteger());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void ByteArray_Test()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, Contract.CheckByteArray());
            Assert.AreEqual(1002305890, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Char_Test()
        {
            Assert.AreEqual(new BigInteger('n'), Contract.CheckChar());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void String_Test()
        {
            Assert.AreEqual("neo", Contract.CheckString());
            Assert.AreEqual(1002060130, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger('e'), Contract.CheckStringIndex("neo", 1));
            Assert.AreEqual(1003109380, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger('o'), Contract.CheckStringIndex("neo", 2));
            Assert.AreEqual(1004158630, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void ArrayObj_Test()
        {
            var item = Contract.CheckArrayObj()!;
            Assert.AreEqual(1002121600, Engine.FeeConsumed.Value);

            Assert.AreEqual(1, item.Count);
            Assert.AreEqual("neo", (item[0] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Enum_Test()
        {
            Assert.AreEqual(new Integer(5), Contract.CheckEnum());
            Assert.AreEqual(1002059920, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Class_Test()
        {
            var item = Contract.CheckClass();
            Assert.AreEqual(1002632920, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual(1, ((Array)item).Count);
            Assert.AreEqual("neo", (((Array)item)[0] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Struct_Test()
        {
            var item = Contract.CheckStruct();
            Assert.AreEqual(1002571870, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType(item, typeof(Struct));
            Assert.AreEqual(1, ((Struct)item).Count);
            Assert.AreEqual("neo", (((Struct)item)[0] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Tuple_Test()
        {
            var item = Contract.CheckTuple()!;
            Assert.AreEqual(1002552490, Engine.FeeConsumed.Value);
            Assert.AreEqual(2, item.Count);
            Assert.AreEqual("neo", (item[0] as ByteString)?.GetString());
            Assert.AreEqual("smart economy", (item[1] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Tuple2_Test()
        {
            var item = Contract.CheckTuple2()!;
            Assert.AreEqual(1002554530, Engine.FeeConsumed.Value);
            Assert.AreEqual(2, item.Count);
            Assert.AreEqual("neo", (item[0] as ByteString)?.GetString());
            Assert.AreEqual("smart economy", (item[1] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Event_Test()
        {
            // Prepare

            Engine.SetTransactionSigners(UInt160.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff01"));
            var hash = Engine.GetDeployHash(Contract_Types.Nef, Contract_Types.Manifest);

            var notifications = new List<string>();
            var delEvent = new Contract_Types.delDummyEvent(notifications.Add);

            // Deploy because notify require a contract

            var result = Contract.Create(Contract_Types.Nef.ToArray(), Contract_Types.Manifest.ToJson().ToString());
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(hash.ToArray(), ((result as Array)![2] as ByteString)?.GetSpan().ToArray());

            var cnew = Engine.FromHash<Contract_Types>(hash, true);
            cnew.OnDummyEvent += delEvent;
            result = Contract.Call(hash, "checkEvent", (int)CallFlags.All, []);
            cnew.OnDummyEvent -= delEvent;

            Assert.IsNull(result);
            Assert.AreEqual(1, notifications.Count);
            Assert.AreEqual("neo", notifications.Last());
        }

        [TestMethod]
        public void Lambda_Test()
        {
            var item = Contract.CheckLambda();
            Assert.AreEqual(1002060010, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }

        [TestMethod]
        public void Delegate_Test()
        {
            var item = Contract.CheckDelegate();
            Assert.AreEqual(1002060010, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }

        [TestMethod]
        public void Nameof_test()
        {
            Assert.AreEqual("checkNull", Contract.CheckNameof());
            Assert.AreEqual(1002060130, Engine.FeeConsumed.Value);
        }
    }
}
