using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography;
using Neo.Network.P2P.Payloads;
using Neo.VM;
using Neo.VM.Types;
using Neo.Wallets;
using System.Linq;
using System.Security.Cryptography;
using Neo.Network.P2P;
using Neo.SmartContract.TestEngine;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class CryptoTest
    {
        private KeyPair _key = null;
        private TestEngine.TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine.TestEngine(TriggerType.Application, new Transaction()
            {
                Attributes = System.Array.Empty<TransactionAttribute>(),
                Script = System.Array.Empty<byte>(),
                Signers = new Signer[] { new Signer() { Account = UInt160.Zero } },
                Witnesses = System.Array.Empty<Witness>(),
                NetworkFee = 1,
                Nonce = 2,
                SystemFee = 3,
                ValidUntilBlock = 4,
                Version = 5
            }, new TestDataCache());
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
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("SHA256", "asd");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Buffer));
            Assert.AreEqual("688787d8ff144c502c7f5cffaafe2cc588d86079f9de88304c26b0cb99ce91c6", item.GetSpan().ToArray().ToHexString());
        }

        [TestMethod]
        public void Test_Murmur32()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("murmur32", "asd", 2);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Buffer));
            Assert.AreEqual("2ad58504", item.GetSpan().ToArray().ToHexString());
        }

        [TestMethod]
        public void Test_RIPEMD160()
        {
            _engine.Reset();
            var str = System.Text.Encoding.Default.GetBytes("hello world");
            var result = _engine.ExecuteTestCaseStandard("RIPEMD160", str);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Buffer));
            Assert.AreEqual("98c615784ccb5fe5936fbc0cbe9dfdb408d92f0f", item.GetSpan().ToArray().ToHexString());
        }

        [TestMethod]
        public void Test_VerifySignatureWithMessage()
        {
            byte[] signature = Crypto.Sign(_engine.ScriptContainer.GetSignData(ProtocolSettings.Default.Network),
                _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignatureWithMessage",
                new ByteString(System.Array.Empty<byte>()),
                new ByteString(_key.PublicKey.EncodePoint(true)), new ByteString(signature));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.GetBoolean());

            // True

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("secp256r1VerifySignatureWithMessage",
                new ByteString(_engine.ScriptContainer.GetSignData(ProtocolSettings.Default.Network)),
                new ByteString(_key.PublicKey.EncodePoint(true)), new ByteString(signature));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.GetBoolean());
        }

        [TestMethod]
        public void Test_Bls12381Serialize_And_Deserialize()
        {
            _engine.Reset();
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();
            var result = _engine.ExecuteTestCaseStandard("bls12381Deserialize", g1);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            _engine.Reset();
            var result2 = _engine.ExecuteTestCaseStandard("bls12381Serialize", item);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result2.Count);
        }

        [TestMethod]
        public void Test_Bls12381Equal()
        {
            _engine.Reset();
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();
            var result1 = _engine.ExecuteTestCaseStandard("bls12381Deserialize", g1);
            var item1 = result1.Pop();

            _engine.Reset();
            var result2 = _engine.ExecuteTestCaseStandard("bls12381Deserialize", g1);
            var item2 = result2.Pop();

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("bls12381Equal", item1, item2);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void Test_Bls12381Add()
        {
            _engine.Reset();
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();
            var result1 = _engine.ExecuteTestCaseStandard("bls12381Deserialize", g1);
            var item1 = result1.Pop();

            _engine.Reset();
            var result2 = _engine.ExecuteTestCaseStandard("bls12381Deserialize", g1);
            var item2 = result2.Pop();

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("bls12381Add", item1, item2);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void Test_Bls12381Mul()
        {
            _engine.Reset();
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();
            byte[] mul = "0300000000000000000000000000000000000000000000000000000000000000".ToLower().HexToBytes();
            var result1 = _engine.ExecuteTestCaseStandard("bls12381Deserialize", g1);
            var item1 = result1.Pop();

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("bls12381Mul", item1, mul, true);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void Test_Bls12381Pairing()
        {
            _engine.Reset();
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();
            byte[] g2 = "93e02b6052719f607dacd3a088274f65596bd0d09920b61ab5da61bbdc7f5049334cf11213945d57e5ac7d055d042b7e024aa2b2f08f0a91260805272dc51051c6e47ad4fa403b02b4510b647ae3d1770bac0326a805bbefd48056c8c121bdb8".ToLower().HexToBytes();
            var result1 = _engine.ExecuteTestCaseStandard("bls12381Deserialize", g1);
            var item1 = result1.Pop();

            _engine.Reset();
            var result2 = _engine.ExecuteTestCaseStandard("bls12381Deserialize", g2);
            var item2 = result2.Pop();

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("bls12381Pairing", item1, item2);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);
        }
    }
}
