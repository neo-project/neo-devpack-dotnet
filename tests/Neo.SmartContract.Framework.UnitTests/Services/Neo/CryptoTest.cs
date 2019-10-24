using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
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
                Cosigners = new Cosigner[0],
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
        public void Test_VerifySignature()
        {
            byte[] signature = Crypto.Default.Sign(_engine.ScriptContainer.GetHashData(),
                _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("VerifySignature",
                new ByteArray(_key.PublicKey.EncodePoint(true)), new ByteArray(new byte[64]));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.GetBoolean());

            // True

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("VerifySignature",
                new ByteArray(_key.PublicKey.EncodePoint(true)), new ByteArray(signature));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.GetBoolean());
        }

        [TestMethod]
        public void Test_VerifySignatures()
        {
            byte[] signature = Crypto.Default.Sign(_engine.ScriptContainer.GetHashData(),
                _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("VerifySignatures",
                new Array(new StackItem[] { new ByteArray(_key.PublicKey.EncodePoint(true)) }),
                new Array(new StackItem[] { new ByteArray(new byte[64]) }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.GetBoolean());

            // True

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("VerifySignatures",
                new Array(new StackItem[] { new ByteArray(_key.PublicKey.EncodePoint(true)) }),
                new Array(new StackItem[] { new ByteArray(signature) }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.GetBoolean());
        }

        [TestMethod]
        public void Test_VerifySignatureWithMessage()
        {
            byte[] signature = Crypto.Default.Sign(_engine.ScriptContainer.GetHashData(),
                _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("VerifySignatureWithMessage",
                new ByteArray(new byte[0]),
                new ByteArray(_key.PublicKey.EncodePoint(true)), new ByteArray(signature));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.GetBoolean());

            // True

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("VerifySignatureWithMessage",
                new ByteArray(_engine.ScriptContainer.GetHashData()),
                new ByteArray(_key.PublicKey.EncodePoint(true)), new ByteArray(signature));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.GetBoolean());
        }
    }
}
