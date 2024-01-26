using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO;
using Neo.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.TestEngine;
using Neo.VM;
using Neo.VM.Types;
using Neo.Wallets;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            Assert.IsTrue(_engine.AddEntryScript<Contract_Types>().Success);
        }

        #region Unsupported Types

        [TestMethod]
        public void float_Test()
        {
            using var testengine = new TestEngine();
            Assert.IsFalse(testengine.AddEntryScript<Contract_Types_Float>().Success);
        }

        [TestMethod]
        public void decimal_Test()
        {
            using var testengine = new TestEngine();
            Assert.IsFalse(testengine.AddEntryScript<Contract_Types_Decimal>().Success);
        }

        [TestMethod]
        public void double_Test()
        {
            using var testengine = new TestEngine();
            Assert.IsFalse(testengine.AddEntryScript<Contract_Types_Double>().Success);
        }

        #endregion

        [TestMethod]
        public void null_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkNull");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Null));
        }

        [TestMethod]
        public void bool_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkBoolTrue");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(true, item.GetBoolean());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("checkBoolFalse");

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(false, item.GetBoolean());
        }

        [TestMethod]
        public void byteStringConcat_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("concatByteString", "1", "2");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("1212", item.GetString());
        }

        [TestMethod]
        public void bigInteer_Test()
        {
            using var _engine = new TestEngine(snapshot: new TestDataCache());
            _engine.AddEntryScript<Contract_Types_BigInteger>();

            // static vars

            var result = _engine.ExecuteTestCaseStandard("zero");
            var item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(BigInteger.Zero, ((Integer)item).GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("one");
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(BigInteger.One, ((Integer)item).GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("minusOne");
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(BigInteger.MinusOne, ((Integer)item).GetInteger());

            // Parse

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("parse", "456");
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(456, item.GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("convertFromChar");
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(65, item.GetInteger());
        }

        [TestMethod]
        public void toAddress_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript<Contract_Types>();
            var result = testengine.ExecuteTestCaseStandard("toAddress", "820944cfdc70976602d71b0091445eedbc661bc5".HexToBytes().Reverse().ToArray(), 53);
            Assert.AreEqual("NdtB8RXRmJ7Nhw1FPTm7E6HoDZGnDw37nf", result.Pop().GetString());
        }

        [TestMethod]
        public void checkEnumArg_Test()
        {
            var methods = _engine.Manifest.Abi.Methods;
            var checkEnumArg = methods.Where(u => u.Name == "checkEnumArg").FirstOrDefault();
            Assert.AreEqual(new JArray(checkEnumArg.Parameters.Select(u => u.ToJson()).ToArray()).ToString(false), @"[{""name"":""arg"",""type"":""Integer""}]");
        }

        [TestMethod]
        public void checkBoolString_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkBoolString", true);
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual(true.ToString(), item.GetString());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("checkBoolString", false);
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual(false.ToString(), item.GetString());
        }

        [TestMethod]
        public void sbyte_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkSbyte");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void byte_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkByte");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void short_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkShort");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void ushort_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkUshort");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void int_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkInt");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void uint_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkUint");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void long_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkLong");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void ulong_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkUlong");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void bigInteger_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkBigInteger");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void byteArray_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkByteArray");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Buffer));
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, ((VM.Types.Buffer)item).GetSpan().ToArray());
        }

        [TestMethod]
        public void char_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkChar");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual((int)'n', ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void string_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkString");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("neo", item.GetString());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("checkStringIndex", "neo", 1);
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual("e", item.GetString());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("checkStringIndex", "neo", 2);
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual("o", item.GetString());
        }

        [TestMethod]
        public void arrayObj_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkArrayObj");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Array));
            Assert.AreEqual(1, ((VM.Types.Array)item).Count);
            Assert.AreEqual("neo", (((VM.Types.Array)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void enum_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkEnum");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void class_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkClass");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Array));
            Assert.AreEqual(1, ((VM.Types.Array)item).Count);
            Assert.AreEqual("neo", (((VM.Types.Array)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void struct_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkStruct");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Struct));
            Assert.AreEqual(1, ((Struct)item).Count);
            Assert.AreEqual("neo", (((Struct)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void tuple_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkTuple");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Array));
            Assert.AreEqual(2, ((VM.Types.Array)item).Count);
            Assert.AreEqual("neo", (((VM.Types.Array)item)[0] as ByteString).GetString());
            Assert.AreEqual("smart economy", (((VM.Types.Array)item)[1] as ByteString).GetString());
        }

        [TestMethod]
        public void tuple2_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkTuple2");
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Array));
            Assert.AreEqual(2, ((VM.Types.Array)item).Count);
            Assert.AreEqual("neo", (((VM.Types.Array)item)[0] as ByteString).GetString());
            Assert.AreEqual("smart economy", (((VM.Types.Array)item)[1] as ByteString).GetString());
        }

        [TestMethod]
        public void event_Test()
        {
            var system = new NeoSystem(TestProtocolSettings.Default);
            using var _engine = new TestEngine(verificable: new Transaction()
            {
                Signers = new Signer[] { new Signer() { Account = UInt160.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff01") } },
                Witnesses = System.Array.Empty<Witness>(),
                Attributes = System.Array.Empty<TransactionAttribute>()
            },
            snapshot: new TestDataCache(system.GenesisBlock),
            persistingBlock: system.GenesisBlock);

            _engine.AddEntryScript<Contract_Types>();

            var manifest = _engine.Manifest;
            var nef = new NefFile() { Script = _engine.Nef.Script, Compiler = _engine.Nef.Compiler, Source = _engine.Nef.Source, Tokens = _engine.Nef.Tokens };
            nef.CheckSum = NefFile.ComputeChecksum(nef);

            var hash = SmartContract.Helper.GetContractHash((_engine.ScriptContainer as Transaction).Sender, nef.CheckSum, manifest.Name);

            // Deploy because notify require a contract

            _engine.Reset();

            var result = _engine.ExecuteTestCaseStandard("create", nef.ToArray(), manifest.ToJson().ToString());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count); // Hash can be retrived here

            _engine.Reset();

            result = _engine.ExecuteTestCaseStandard("call", hash.ToArray(), "checkEvent", (int)CallFlags.All, new Array());
            //result = _engine.ExecuteTestCaseStandard("checkEvent");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(Null.Null, result.Pop());
            Assert.AreEqual(2, _engine.Notifications.Count);

            var item = _engine.Notifications.Last();

            Assert.AreEqual(1, item.State.Count);
            Assert.AreEqual("dummyEvent", item.EventName);
            Assert.AreEqual("neo", (item.State[0] as ByteString).GetString());
        }

        [TestMethod]
        public void lambda_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkLambda");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }

        [TestMethod]
        public void delegate_Test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkDelegate");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }

        [TestMethod]
        public void UInt160_ValidateAddress()
        {
            var address = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            using var testengine = new TestEngine();
            testengine.AddEntryScript<Contract_UIntTypes>();

            // True

            var result = testengine.ExecuteTestCaseStandard("validateAddress", address.ToArray());
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsTrue(item.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("validateAddress", new ByteString(address.ToArray()));
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsTrue(item.GetBoolean());

            // False

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("validateAddress", new byte[1] { 1 }.Concat(address.ToArray()).ToArray());
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsFalse(item.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("validateAddress", BigInteger.One);
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsFalse(item.GetBoolean());

            testengine.Reset();
            testengine.ExecuteTestCaseStandard("validateAddress", StackItem.Null);
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("validateAddress", new VM.Types.Array());
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsFalse(item.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("validateAddress", new Struct());
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsFalse(item.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("validateAddress", new Map());
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsFalse(item.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("validateAddress", StackItem.True);
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsFalse(item.GetBoolean());
        }

        [TestMethod]
        public void UInt160_equals_test()
        {
            var owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);
            var notOwner = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            using var testengine = new TestEngine();
            testengine.AddEntryScript<Contract_UIntTypes>();

            var result = testengine.ExecuteTestCaseStandard("checkOwner", owner.ToArray());
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsTrue(item.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkOwner", notOwner.ToArray());
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsFalse(item.GetBoolean());
        }

        [TestMethod]
        public void UInt160_equals_zero_test()
        {
            var zero = UInt160.Zero;
            var notZero = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            using var testengine = new TestEngine();
            testengine.AddEntryScript<Contract_UIntTypes>();
            var result = testengine.ExecuteTestCaseStandard("checkZeroStatic", zero.ToArray());
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsTrue(item.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkZeroStatic", notZero.ToArray());
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsFalse(item.GetBoolean());
        }

        [TestMethod]
        public void UInt160_byte_array_construct()
        {
            var notZero = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            using var testengine = new TestEngine();
            testengine.AddEntryScript<Contract_UIntTypes>();

            var result = testengine.ExecuteTestCaseStandard("constructUInt160", notZero.ToArray());
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsTrue(item is ByteString);
            var received = new UInt160(((ByteString)item).GetSpan());
            Assert.AreEqual(received, notZero);
        }

        [TestMethod]
        public void ECPoint_test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript<Contract_Types_ECPoint>();

            var result = testengine.ExecuteTestCaseStandard("isValid", "0102");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsTrue(item is Boolean b1 && !b1.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("isValid", new byte[33]);
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsTrue(item is Boolean b2 && b2.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("isValid", false);
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsTrue(item is Boolean b3 && !b3.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("ecpoint2String");
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsTrue(item is ByteString s1);
            Assert.AreEqual(item.GetSpan().ToHexString(), "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("ecpointReturn");
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsTrue(item is ByteString s2);
            Assert.AreEqual(item.GetSpan().ToHexString(), "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("ecpoint2ByteArray");
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsTrue(item is Buffer bf1);
            Assert.AreEqual(item.GetSpan().ToHexString(), "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
        }

        [TestMethod]
        public void Nameof_test()
        {
            var result = _engine.ExecuteTestCaseStandard("checkNameof");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsTrue(item is ByteString);
            Assert.AreEqual(item.GetString(), "checkNull");
        }
    }
}
