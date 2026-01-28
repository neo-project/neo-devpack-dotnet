// Copyright (C) 2015-2026 The Neo Project.
//
// CryptoTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography;
using Neo.Network.P2P;
using Neo.SmartContract.Testing;
using Neo.Wallets;
using System;
using System.Security.Cryptography;
using System.Text;
using Neo.Extensions;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class CryptoTest : DebugAndTestBase<Contract_Crypto>
    {
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
                Contract.SHA256(Encoding.UTF8.GetBytes("asd"))!.ToHexString());
        }

        [TestMethod]
        public void Test_Murmur32()
        {
            Assert.AreEqual("2ad58504",
               Contract.Murmur32(Encoding.UTF8.GetBytes("asd"), 2)!.ToHexString());
        }

        [TestMethod]
        public void Test_RIPEMD160()
        {
            Assert.AreEqual("98c615784ccb5fe5936fbc0cbe9dfdb408d92f0f",
              Contract.RIPEMD160(Encoding.UTF8.GetBytes("hello world"))!.ToHexString());
        }

        [TestMethod]
        public void Test_VerifySignatureWithMessage()
        {
            // secp256r1 with SHA256 hash

            var key = GenerateKey(32);
            var data = Engine.Transaction.GetSignData(ProtocolSettings.Default.Network);
            var signature = Crypto.Sign(data, key.PrivateKey);

            // Check

            Assert.IsFalse(Contract.Secp256r1VerifySignatureWithMessage([], key.PublicKey, signature));
            Assert.IsTrue(Contract.Secp256r1VerifySignatureWithMessage(data, key.PublicKey, signature));

            // secp256r1 with Keccak hash

            var signatureKeccak = Crypto.Sign(data, key.PrivateKey, Cryptography.ECC.ECCurve.Secp256r1, Cryptography.HashAlgorithm.Keccak256);

            // Check

            Assert.IsFalse(Contract.Secp256r1VerifyKeccakSignatureWithMessage([], key.PublicKey, signatureKeccak));
            Assert.IsTrue(Contract.Secp256r1VerifyKeccakSignatureWithMessage(data, key.PublicKey, signatureKeccak));

            // secp256k1 with SHA256 hash

            var pubkey = Cryptography.ECC.ECCurve.Secp256k1.G * key.PrivateKey;

            signature = Crypto.Sign(data, key.PrivateKey, Cryptography.ECC.ECCurve.Secp256k1);

            // Check

            Assert.IsFalse(Contract.Secp256k1VerifySignatureWithMessage([], pubkey, signature));
            Assert.IsTrue(Contract.Secp256k1VerifySignatureWithMessage(data, pubkey, signature));

            // secp256k1 with Keccak hash

            signature = Crypto.Sign(data, key.PrivateKey, Cryptography.ECC.ECCurve.Secp256k1, Cryptography.HashAlgorithm.Keccak256);

            // Check

            Assert.IsFalse(Contract.Secp256k1VerifyKeccakSignatureWithMessage([], pubkey, signature));
            Assert.IsTrue(Contract.Secp256k1VerifyKeccakSignatureWithMessage(data, pubkey, signature));
        }

        [TestMethod]
        public void Test_RecoverSecp256K1()
        {
            // Test vectors adapted from neo's UT_Crypto / UT_CryptoLib
            var messageHash1 = "5ae8317d34d1e595e3fa7247db80c0af4320cce1116de187f8f7e2e099c0d8d0".HexToBytes();
            var signature1 = ("45c0b7f8c09a9e1f1cea0c25785594427b6bf8f9f878a8af0b1abbb48e16d092" +
                              "0d8becd0c220f67c51217eecfd7184ef0732481c843857e6bc7fc095c4f6b78801").HexToBytes();
            var expectedPubKey1 = "034a071e8a6e10aada2b8cf39fa3b5fb3400b04e99ea8ae64ceea1a977dbeaf5d5".HexToBytes();

            // 65-byte signature where v is in [0..3]
            CollectionAssert.AreEqual(expectedPubKey1, Contract.RecoverSecp256K1(messageHash1, signature1));

            // 65-byte signature with Ethereum-style v in [27..30]
            var signature1EthV = new byte[signature1.Length];
            Buffer.BlockCopy(signature1, 0, signature1EthV, 0, signature1.Length);
            signature1EthV[64] += 27;
            CollectionAssert.AreEqual(expectedPubKey1, Contract.RecoverSecp256K1(messageHash1, signature1EthV));

            // 64-byte compact signature (r[32] || yParityAndS[32]) per EIP-2098
            var privateKey = "1234567890123456789012345678901234567890123456789012345678901234".HexToBytes();
            var expectedPubKey2 = (Cryptography.ECC.ECCurve.Secp256k1.G * privateKey).EncodePoint(true);
            var messageHash2 = GetEthereumSignedMessageHash(Encoding.UTF8.GetBytes("It's a small(er) world"));
            var signature2 = new byte[64];
            Buffer.BlockCopy("9328da16089fcba9bececa81663203989f2df5fe1faa6291a45381c81bd17f76".HexToBytes(), 0, signature2, 0, 32);
            Buffer.BlockCopy("939c6d6b623b42da56557e5e734a43dc83345ddfadec52cbe24d0cc64f550793".HexToBytes(), 0, signature2, 32, 32);
            CollectionAssert.AreEqual(expectedPubKey2, Contract.RecoverSecp256K1(messageHash2, signature2));

            // Invalid inputs should return null
            Assert.IsNull(Contract.RecoverSecp256K1(messageHash1[..31], signature1));
            Assert.IsNull(Contract.RecoverSecp256K1(messageHash1, signature1[..63]));

            var invalidRecoverySignature = new byte[signature1.Length];
            Buffer.BlockCopy(signature1, 0, invalidRecoverySignature, 0, signature1.Length);
            invalidRecoverySignature[64] = 29;
            Assert.IsNull(Contract.RecoverSecp256K1(messageHash1, invalidRecoverySignature));
        }

        [TestMethod]
        public void Test_VerifyWithEd25519()
        {
            // Generate a fresh Ed25519 key pair
            byte[] privateKey = Ed25519.GenerateKeyPair();
            byte[] publicKey = Ed25519.GetPublicKey(privateKey);
            byte[] message = Encoding.UTF8.GetBytes("Hello, Neo!");
            byte[] signature = Ed25519.Sign(privateKey, message);

            // Verify using Ed25519 directly
            Assert.IsTrue(Ed25519.Verify(publicKey, message, signature));

            // Verify using CryptoLib.VerifyWithEd25519
            Assert.IsTrue(Contract.VerifyWithEd25519(message, publicKey, signature));

            // Test with a different message
            byte[] differentMessage = Encoding.UTF8.GetBytes("Different message");
            Assert.IsFalse(Contract.VerifyWithEd25519(differentMessage, publicKey, signature));

            // Test with an invalid signature
            byte[] invalidSignature = new byte[signature.Length];
            Buffer.BlockCopy(signature, 0, invalidSignature, 0, signature.Length);
            invalidSignature[0] ^= 0x01; // Flip one bit
            Assert.IsFalse(Contract.VerifyWithEd25519(message, publicKey, invalidSignature));
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

        private static byte[] GetEthereumSignedMessageHash(byte[] messageBody)
        {
            var prefix = Encoding.UTF8.GetBytes($"Ethereum Signed Message:\n{messageBody.Length}");
            var message = new byte[1 + prefix.Length + messageBody.Length];
            message[0] = 0x19;
            Buffer.BlockCopy(prefix, 0, message, 1, prefix.Length);
            Buffer.BlockCopy(messageBody, 0, message, 1 + prefix.Length, messageBody.Length);
            return message.Keccak256();
        }
    }
}
