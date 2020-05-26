using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.Cryptography;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.VM;
using Neo.VM.Types;
using Neo.Wallets;
using System.Linq;
using System.Security.Cryptography;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class CryptoTest
    {
        private KeyPair _key = null;
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(TriggerType.Application, new Transaction()
            {
                Attributes = new TransactionAttribute[0],
                Script = new byte[0],
                Sender = UInt160.Zero,
                Witnesses = new Witness[0],
                NetworkFee = 1,
                Nonce = 2,
                SystemFee = 3,
                ValidUntilBlock = 4,
                Version = 5
            });
            _engine.AddEntryScript("./TestClasses/Contract_Crypto.cs");
            _key = GenerateKey(32);
        }

        public static KeyPair GenerateKey(int privateKeyLength)
        {
            byte[] privateKey = new byte[privateKeyLength];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(privateKey);
            }
            return new KeyPair(privateKey);
        }

        [TestMethod]
        public void Test_SHA256()
        {
            var data = _engine.ScriptContainer.GetHashData();
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("sHA256", data);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("293ba9cd0c05e23da15e39d29bcb8edfa5b2eeb29163a325c3229e81feed3d11", item.GetSpan().ToArray().ToHexString());
        }

        [TestMethod]
        public void Test_VerifySignature()
        {
            byte[] signature = Crypto.Sign(_engine.ScriptContainer.GetHashData(),
                _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignature",
                new ByteString(_key.PublicKey.EncodePoint(true)), new ByteString(new byte[64]));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());

            // True

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignature",
                new ByteString(_key.PublicKey.EncodePoint(true)), new ByteString(signature));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());
        }

        [TestMethod]
        public void Test_VerifySignatures()
        {
            byte[] signature = Crypto.Sign(_engine.ScriptContainer.GetHashData(),
                _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignatures",
                new Array(new StackItem[] { new ByteString(_key.PublicKey.EncodePoint(true)) }),
                new Array(new StackItem[] { new ByteString(new byte[64]) }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());

            // True

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignatures",
                new Array(new StackItem[] { new ByteString(_key.PublicKey.EncodePoint(true)) }),
                new Array(new StackItem[] { new ByteString(signature) }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());
        }

        [TestMethod]
        public void Test_VerifySignaturesWithMessage()
        {
            byte[] signature = Crypto.Sign(_engine.ScriptContainer.GetHashData(),
                _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignaturesWithMessage",
                new ByteString(new byte[0]),
                new Array(new StackItem[] { new ByteString(_key.PublicKey.EncodePoint(true)) }),
                new Array(new StackItem[] { new ByteString(new byte[64]) }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());

            // True

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignaturesWithMessage",
                new ByteString(_engine.ScriptContainer.GetHashData()),
                new Array(new StackItem[] { new ByteString(_key.PublicKey.EncodePoint(true)) }),
                new Array(new StackItem[] { new ByteString(signature) }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());
        }

        [TestMethod]
        public void Test_VerifySignatureWithMessage()
        {
            byte[] signature = Crypto.Sign(_engine.ScriptContainer.GetHashData(),
                _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignatureWithMessage",
                new ByteString(new byte[0]),
                new ByteString(_key.PublicKey.EncodePoint(true)), new ByteString(signature));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());

            // True

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignatureWithMessage",
                new ByteString(_engine.ScriptContainer.GetHashData()),
                new ByteString(_key.PublicKey.EncodePoint(true)), new ByteString(signature));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());
        }
    }
}
