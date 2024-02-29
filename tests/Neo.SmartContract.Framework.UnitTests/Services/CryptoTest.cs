using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography;
using Neo.Network.P2P;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.Wallets;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class CryptoTest : TestBase<Contract_Crypto>
    {
        public CryptoTest() : base(Contract_Crypto.Nef, Contract_Crypto.Manifest) { }

        public static KeyPair GenerateKey(int privateKeyLength = 32)
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
            Assert.AreEqual("688787d8ff144c502c7f5cffaafe2cc588d86079f9de88304c26b0cb99ce91c6",
                Contract.SHA256(Encoding.UTF8.GetBytes("asd")).ToHexString());
        }

        [TestMethod]
        public void Test_Murmur32()
        {
            Assert.AreEqual("2ad58504",
               Contract.Murmur32(Encoding.UTF8.GetBytes("asd"), 2).ToHexString());
        }

        [TestMethod]
        public void Test_RIPEMD160()
        {
            Assert.AreEqual("98c615784ccb5fe5936fbc0cbe9dfdb408d92f0f",
              Contract.RIPEMD160(Encoding.UTF8.GetBytes("hello world")).ToHexString());
        }

        [TestMethod]
        public void Test_VerifySignatureWithMessage()
        {
            var _key = GenerateKey(32);
            var data = Engine.Transaction.GetSignData(ProtocolSettings.Default.Network);
            var signature = Crypto.Sign(data, _key.PrivateKey, _key.PublicKey.EncodePoint(false).Skip(1).ToArray());

            // False

            Assert.IsFalse(Contract.Secp256r1VerifySignatureWithMessage(System.Array.Empty<byte>(), _key.PublicKey, signature));

            // True

            Assert.IsTrue(Contract.Secp256r1VerifySignatureWithMessage(data, _key.PublicKey, signature));
        }

        [TestMethod]
        public void Test_Bls12381Serialize_And_Deserialize()
        {
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();

            var item = Contract.Bls12381Deserialize(g1);
            Assert.IsNotNull(item);
            Assert.IsNotNull(Contract.Bls12381Serialize(item));
        }

        [TestMethod]
        public void Test_Bls12381Equal()
        {
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();

            var item1 = Contract.Bls12381Deserialize(g1);
            var item2 = Contract.Bls12381Deserialize(g1);
            var result = Contract.Bls12381Equal(item1, item2);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_Bls12381Add()
        {
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();

            var item1 = Contract.Bls12381Deserialize(g1);
            var item2 = Contract.Bls12381Deserialize(g1);
            var result = Contract.Bls12381Add(item1, item2);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_Bls12381Mul()
        {
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();
            byte[] mul = "0300000000000000000000000000000000000000000000000000000000000000".ToLower().HexToBytes();

            var item1 = Contract.Bls12381Deserialize(g1);
            var result = Contract.Bls12381Mul(item1, mul, true);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_Bls12381Pairing()
        {
            byte[] g1 = "97f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb".ToLower().HexToBytes();
            byte[] g2 = "93e02b6052719f607dacd3a088274f65596bd0d09920b61ab5da61bbdc7f5049334cf11213945d57e5ac7d055d042b7e024aa2b2f08f0a91260805272dc51051c6e47ad4fa403b02b4510b647ae3d1770bac0326a805bbefd48056c8c121bdb8".ToLower().HexToBytes();

            var item1 = Contract.Bls12381Deserialize(g1);
            var item2 = Contract.Bls12381Deserialize(g2);
            var result = Contract.Bls12381Pairing(item1, item2);

            Assert.IsNotNull(result);
        }
    }
}
