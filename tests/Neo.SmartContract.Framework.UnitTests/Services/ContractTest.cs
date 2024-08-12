using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using Array = Neo.VM.Types.Array;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class ContractTest : DebugAndTestBase<Contract_Contract>
    {
        public ContractTest() : base()
        {
            // Ensure also Contract_Create
            TestCleanup.TestInitialize(typeof(Contract_Create));
            TestCleanup.TestInitialize(typeof(Contract_Update));
        }

        [TestMethod]
        public void Test_CreateCallDestroy()
        {
            Engine.Storage.Commit();
            var created = Test_CreateCall();

            // Destroy

            Assert.AreEqual((byte)CallFlags.All, created.GetCallFlags());
            created.Destroy();

            // Check again for failures

            var exception = Assert.ThrowsException<TestException>(() => created.GetCallFlags());
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
            Engine.Storage.Rollback();
        }

        public Contract_Create Test_CreateCall()
        {
            // Create

            Engine.SetTransactionSigners(Bob);
            var hash = Engine.GetDeployHash(Contract_Create.Nef, Contract_Create.Manifest);

            // Create

            var item = Contract.Create(Contract_Create.Nef.ToArray(), Contract_Create.Manifest.ToJson().ToString()) as Array;
            Assert.IsNotNull(item);
            Assert.AreEqual(2, item[0].GetInteger()); // Id
            Assert.AreEqual(0, item[1].GetInteger()); // UpdateCounter
            Assert.AreEqual(hash.ToArray(), item[2]); // Hash
            Assert.AreEqual(Contract_Create.Nef.ToJson().AsString(), item[3].GetSpan().ToArray().AsSerializable<NefFile>().ToJson().AsString()); // Nef

            var ritem = new ContractManifest();
            ((IInteroperable)ritem).FromStackItem(item[4]);
            Assert.AreEqual(Contract_Create.Manifest.ToString(), ritem.ToString()); // Manifest

            var created = Engine.FromHash<Contract_Create>(hash, true);

            // Call

            var item2 = Contract.Call(hash, "oldContract", (byte)CallFlags.All, new List<object>()) as ByteString;

            Assert.IsNotNull(item2);
            Assert.AreEqual(Contract_Contract.Manifest.Name, item2.GetString());

            // getContractById
            item = Contract.Call(hash, "getContractById", (byte)CallFlags.All, new List<object>() { 2 }) as Array;

            Assert.IsNotNull(item);
            Assert.AreEqual(2, item[0].GetInteger()); // Id
            Assert.AreEqual(0, item[1].GetInteger()); // UpdateCounter
            CollectionAssert.AreEqual(hash.ToArray(), item[2].GetSpan().ToArray()); // Hash
            Assert.AreEqual(Contract_Create.Nef.ToJson().AsString(), item[3].GetSpan().ToArray().AsSerializable<NefFile>().ToJson().AsString()); // Nef

            // getContractHashes
            item2 = Contract.Call(hash, "getContractHashes", (byte)CallFlags.All, new List<object>()) as ByteString;
            Assert.IsNotNull(item2);
            CollectionAssert.AreEqual(Contract.Hash.ToArray(), item2.GetSpan().ToArray()); // Hash

            return created;
        }

        [TestMethod]
        public void Test_Update()
        {
            Engine.Storage.Commit();
            var created = Test_CreateCall();

            // Update

            var manifest = Contract_Update.Manifest.ToJson();
            manifest["name"] = Contract_Create.Manifest.Name; // Can't change the name

            Assert.AreEqual(0, Engine.Native.ContractManagement.GetContract(created.Hash)!.UpdateCounter);
            created.Update(Contract_Update.Nef.ToArray(), manifest.ToString());
            Assert.AreEqual(1, Engine.Native.ContractManagement.GetContract(created.Hash)!.UpdateCounter);

            // Call Again

            var update = Engine.FromHash<Contract_Update>(created, true);
            Assert.AreEqual("YES", update.ImUpdated());

            Engine.Storage.Rollback();
        }

        [TestMethod]
        public void Test_CreateStandardAccount()
        {
            // Wrong pubKey

            var exception = Assert.ThrowsException<TestException>(() => Contract.CreateStandardAccount(null));
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
            exception = Assert.ThrowsException<TestException>(() => Contract.CreateStandardAccount(InvalidECPoint.InvalidLength));
            Assert.IsInstanceOfType<IndexOutOfRangeException>(exception.InnerException);
            exception = Assert.ThrowsException<TestException>(() => Contract.CreateStandardAccount(InvalidECPoint.InvalidType));
            Assert.IsInstanceOfType<IndexOutOfRangeException>(exception.InnerException);

            // Good pubKey (compressed)

            var point = ECPoint.FromBytes(new byte[] {
                    0x02, 0x48, 0x6f, 0xd1, 0x57, 0x02, 0xc4, 0x49, 0x0a, 0x26,
                    0x70, 0x31, 0x12, 0xa5, 0xcc, 0x1d, 0x09, 0x23, 0xfd, 0x69,
                    0x7a, 0x33, 0x40, 0x6b, 0xd5, 0xa1, 0xc0, 0x0e, 0x00, 0x13,
                    0xb0, 0x9a, 0x70 }, ECCurve.Secp256r1);

            Assert.AreEqual("0xba7849a7c80dd617227eebebba74194880823850", Contract.CreateStandardAccount(point)?.ToString());

            // Good pubKey (uncompressed)

            point = ECPoint.FromBytes(new byte[] {
                    0x04, 0x48, 0x6f, 0xd1, 0x57, 0x02, 0xc4, 0x49, 0x0a, 0x26,
                    0x70, 0x31, 0x12, 0xa5, 0xcc, 0x1d, 0x09, 0x23, 0xfd, 0x69,
                    0x7a, 0x33, 0x40, 0x6b, 0xd5, 0xa1, 0xc0, 0x0e, 0x00, 0x13,
                    0xb0, 0x9a, 0x70, 0x05, 0x43, 0x6c, 0x08, 0x2c, 0x2c, 0x88,
                    0x08, 0x5b, 0x4b, 0x53, 0xd5, 0x4c, 0x55, 0x66, 0xba, 0x44,
                    0x8d, 0x5c, 0x3e, 0x2a, 0x2a, 0x5c, 0x3a, 0x3e, 0xa5, 0x00,
                    0xe1, 0x40, 0x77, 0x55, 0x9c }, ECCurve.Secp256r1);

            Assert.AreEqual("0xba7849a7c80dd617227eebebba74194880823850", Contract.CreateStandardAccount(point)?.ToString());
        }

        [TestMethod]
        public void Test_GetCallFlags()
        {
            Assert.AreEqual(0b00001111, Contract.GetCallFlags());
        }
    }
}
