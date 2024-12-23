using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types : DebugAndTestBase<Contract_Types>
    {
        [TestMethod]
        public void Null_Test()
        {
            Assert.IsNull(Contract.CheckNull());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Bool_Test()
        {
            Assert.IsTrue(Contract.CheckBoolTrue());
            AssertGasConsumed(984060);
            Assert.IsFalse(Contract.CheckBoolFalse());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void ByteStringConcat_Test()
        {
            Assert.AreEqual("1212", Contract.ConcatByteString([(byte)'1'], [(byte)'2']));
            AssertGasConsumed(1969260);
        }

        [TestMethod]
        public void ToAddress_Test()
        {
            Assert.AreEqual("NdtB8RXRmJ7Nhw1FPTm7E6HoDZGnDw37nf", Contract.ToAddress(UInt160.Parse("820944cfdc70976602d71b0091445eedbc661bc5"), 53));
            AssertGasConsumed(4574880);
        }

        [TestMethod]
        public void CheckEnumArg_Test()
        {
            var methods = Contract_Types.Manifest.Abi.Methods;
            var checkEnumArg = methods.First(u => u.Name == "checkEnumArg");
            Assert.AreEqual(new JArray(checkEnumArg.Parameters.Select(u => u.ToJson()).ToArray<JToken?>()).ToString(false), @"[{""name"":""arg"",""type"":""Integer""}]");

            Contract.CheckEnumArg(5);
            AssertGasConsumed(1046970);
        }

        [TestMethod]
        public void CheckBoolString_Test()
        {
            Assert.AreEqual(true.ToString(), Contract.CheckBoolString(true));
            AssertGasConsumed(1047330);
            Assert.AreEqual(false.ToString(), Contract.CheckBoolString(false));
            AssertGasConsumed(1047330);
        }

        [TestMethod]
        public void Sbyte_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckSbyte());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Byte_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckByte());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Short_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckShort());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Ushort_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckUshort());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Int_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckInt());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Uint_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckUint());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Long_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckLong());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Ulong_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckUlong());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void BigInteger_Test()
        {
            Assert.AreEqual(new BigInteger(5), Contract.CheckBigInteger());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void ByteArray_Test()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, Contract.CheckByteArray());
            AssertGasConsumed(1230030);
        }

        [TestMethod]
        public void Char_Test()
        {
            Assert.AreEqual(new BigInteger('n'), Contract.CheckChar());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void String_Test()
        {
            Assert.AreEqual("neo", Contract.CheckString());
            AssertGasConsumed(984270);
            Assert.AreEqual(new BigInteger('e'), Contract.CheckStringIndex("neo", 1));
            AssertGasConsumed(1049250);
            Assert.AreEqual(new BigInteger('o'), Contract.CheckStringIndex("neo", 2));
            AssertGasConsumed(1049250);
        }

        [TestMethod]
        public void ArrayObj_Test()
        {
            var item = Contract.CheckArrayObj()!;
            AssertGasConsumed(1045740);

            Assert.AreEqual(1, item.Count);
            Assert.AreEqual("neo", (item[0] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Enum_Test()
        {
            Assert.AreEqual(new Integer(5), Contract.CheckEnum());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Class_Test()
        {
            var item = Contract.CheckClass();
            AssertGasConsumed(1293840);
            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual(1, ((Array)item).Count);
            Assert.AreEqual("neo", (((Array)item)[0] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Struct_Test()
        {
            var item = Contract.CheckStruct();
            AssertGasConsumed(1293840);
            Assert.IsInstanceOfType(item, typeof(Struct));
            Assert.AreEqual(1, ((Struct)item).Count);
            Assert.AreEqual("neo", (((Struct)item)[0] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Tuple_Test()
        {
            var item = Contract.CheckTuple()!;
            AssertGasConsumed(1045980);
            Assert.AreEqual(2, item.Count);
            Assert.AreEqual("neo", (item[0] as ByteString)?.GetString());
            Assert.AreEqual("smart economy", (item[1] as ByteString)?.GetString());
        }

        [TestMethod]
        public void Tuple2_Test()
        {
            var item = Contract.CheckTuple2()!;
            AssertGasConsumed(1048020);
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
            var delEvent = new Contract_Types.delDummyEvent(notifications.Add!);

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
            AssertGasConsumed(984150);
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }

        [TestMethod]
        public void Delegate_Test()
        {
            var item = Contract.CheckDelegate();
            AssertGasConsumed(984150);
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }

        [TestMethod]
        public void Nameof_Test()
        {
            Assert.AreEqual("checkNull", Contract.CheckNameof());
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void CheckEvent_Test()
        {
            var notifications = new List<string>();
            var delEvent = new Contract_Types.delDummyEvent(notifications.Add!);

            Contract.OnDummyEvent += delEvent;
            Contract.CheckEvent();
            Contract.OnDummyEvent -= delEvent;
            Assert.AreEqual(1, notifications.Count);
            Assert.AreEqual("neo", notifications.Last());
            AssertGasConsumed(2029020);
        }
    }
}
