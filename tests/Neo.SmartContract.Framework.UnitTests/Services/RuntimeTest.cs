using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class RuntimeTest : DebugAndTestBase<Contract_Runtime>
    {
        static RuntimeTest()
        {
            // We need a deterministic deployer for Random method

            Alice = new Network.P2P.Payloads.Signer()
            {
                Account = UInt160.Parse("0102030405060708090A0102030405060708090A"),
                Scopes = Network.P2P.Payloads.WitnessScope.CalledByEntry
            };
        }

        [TestMethod]
        public void Test_InvocationCounter()
        {
            // We need a new TestEngine because invocationCounter it's shared between them

            Script script;
            using (ScriptBuilder sb = new())
            {
                // First
                sb.EmitDynamicCall(Contract.Hash, "getInvocationCounter");
                // Second
                sb.EmitDynamicCall(Contract.Hash, "getInvocationCounter");
                // Sum
                sb.Emit(OpCode.ADD);
                script = sb.ToArray();
            }

            // Check

            var item = Engine.Execute(script);
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(1 + 2, item.GetInteger());
        }

        [TestMethod]
        public void Test_LoadScript()
        {
            Assert.AreEqual(4, Contract.DynamicSum(1, 3));
        }

        [TestMethod]
        public void Test_Time()
        {
            Assert.AreEqual((ulong)Engine.PersistingBlock.Timestamp.TotalMilliseconds, Contract.GetTime());
        }

        [TestMethod]
        public void Test_Platform()
        {
            Assert.AreEqual("NEO", Contract.GetPlatform());
        }

        [TestMethod]
        public void Test_Trigger()
        {
            Assert.AreEqual((byte)TriggerType.Application, Contract.GetTrigger());
        }

        [TestMethod]
        public void Test_Random()
        {
            Engine.SetTransactionSigners(Alice);
            Engine.Transaction.Nonce = 0x01020304;
            Engine.PersistingBlock.Nonce = 0x01020304;
            Assert.AreEqual(BigInteger.Parse("140181351494432352371728933832694804614"), Contract.GetRandom());
        }

        [TestMethod]
        public void Test_GetNetwork()
        {
            Assert.AreEqual(BigInteger.Parse("860833102"), Contract.GetNetwork());
        }

        [TestMethod]
        public void Test_GetAddressVersion()
        {
            Assert.AreEqual((BigInteger)ProtocolSettings.Default.AddressVersion, Contract.GetAddressVersion());
        }

        [TestMethod]
        public void Test_GasLeft()
        {
            Assert.AreEqual(1999015490, Contract.GetGasLeft());
        }

        [TestMethod]
        public void Test_Log()
        {
            Contract.Log("LogTest");
            AssertLogs("LogTest");
        }

        [TestMethod]
        public void Test_CheckWitness()
        {
            // True

            var signer = Testing.TestEngine.GetNewSigner();

            Engine.SetTransactionSigners(signer);
            Assert.IsTrue(Contract.CheckWitness(signer.Account));

            // False

            Engine.SetTransactionSigners(UInt160.Zero);
            Assert.IsFalse(Contract.CheckWitness(signer.Account));
        }

        [TestMethod]
        public void Test_GetNotificationsCount()
        {
            // We need the validator sign because we will transfer NEO

            Engine.SetTransactionSigners(Engine.ValidatorsAddress);

            Script script;
            using (ScriptBuilder sb = new())
            {
                sb.EmitDynamicCall(Contract.Hash, "getNotificationsCount", new object?[] { null });
                script = sb.ToArray();
            }

            Assert.AreEqual(0, Engine.Execute(script).GetInteger());

            using (ScriptBuilder sb = new())
            {
                // One notification to UInt160
                sb.EmitDynamicCall(Engine.Native.NEO.Hash, "transfer", Engine.ValidatorsAddress, UInt160.Zero, 123, null);
                // One notification to Contract
                sb.EmitDynamicCall(Engine.Native.NEO.Hash, "transfer", Engine.ValidatorsAddress, Alice.Account, 1234, null);
                // Consume only contract
                sb.EmitDynamicCall(Contract.Hash, "getNotificationsCount", Engine.Native.NEO.Hash);
                script = sb.ToArray();
            }

            Assert.AreEqual(2, Engine.Execute(script).GetInteger());

            using (ScriptBuilder sb = new())
            {
                // One notification to UInt160
                sb.EmitDynamicCall(Engine.Native.NEO.Hash, "transfer", Engine.ValidatorsAddress, UInt160.Zero, 123, null);
                // One notification to Contract
                sb.EmitDynamicCall(Engine.Native.NEO.Hash, "transfer", Engine.ValidatorsAddress, Alice.Account, 1234, null);
                // Consume all
                sb.EmitDynamicCall(Contract.Hash, "getNotificationsCount", Alice.Account);
                script = sb.ToArray();
            }

            Assert.AreEqual(0, Engine.Execute(script).GetInteger());
        }

        [TestMethod]
        public void Test_GetNotifications()
        {
            // We need the validator sign because we will transfer NEO

            Engine.SetTransactionSigners(Engine.ValidatorsAddress);

            Script script;
            using (ScriptBuilder sb = new())
            {
                // One notification to UInt160
                sb.EmitDynamicCall(Engine.Native.NEO.Hash, "transfer", Engine.ValidatorsAddress, UInt160.Zero, 123, null);
                // One notification to Contract
                sb.EmitDynamicCall(Engine.Native.NEO.Hash, "transfer", Engine.ValidatorsAddress, Alice.Account, 1234, null);
                // Consume only contract
                sb.EmitDynamicCall(Contract.Hash, "getNotifications", Engine.Native.NEO.Hash);
                script = sb.ToArray();
            }

            Assert.AreEqual(123 + 1234, Engine.Execute(script).GetInteger());

            using (ScriptBuilder sb = new())
            {
                // One notification to UInt160
                sb.EmitDynamicCall(Engine.Native.NEO.Hash, "transfer", Engine.ValidatorsAddress, UInt160.Zero, 123, null);
                // One notification to Contract
                sb.EmitDynamicCall(Engine.Native.NEO.Hash, "transfer", Engine.ValidatorsAddress, Alice.Account, 1234, null);
                // Consume all
                sb.EmitDynamicCall(Contract.Hash, "getAllNotifications");
                script = sb.ToArray();
            }

            Assert.AreEqual(123 + 1234, Engine.Execute(script).GetInteger());
        }

        [TestMethod]
        public void Test_GetTransactionHash()
        {
            var hash = Contract.GetTransactionHash();
            Assert.AreEqual(Engine.Transaction.Hash.ToString(), hash!.ToString());
        }

        [TestMethod]
        public void Test_GetTransactionVersion()
        {
            Assert.AreEqual(Engine.Transaction.Version, Contract.GetTransactionVersion());
        }

        [TestMethod]
        public void Test_GetTransactionNonce()
        {
            Assert.AreEqual(Engine.Transaction.Nonce, Contract.GetTransactionNonce());
        }

        [TestMethod]
        public void Test_GetTransactionSender()
        {
            Assert.AreEqual(Engine.Sender.ToString(), Contract.GetTransactionSender()!.ToString());
        }

        [TestMethod]
        public void Test_GetTransaction()
        {
            var tx = (Testing.Native.Models.Transaction)(Contract.GetTransaction() as StackItem)!.ConvertTo(typeof(Testing.Native.Models.Transaction))!;

            Assert.AreEqual(Engine.Transaction.Nonce, tx.Nonce);
            Assert.AreEqual(Engine.Transaction.NetworkFee, tx.NetworkFee);
            Assert.AreEqual(Engine.Transaction.SystemFee, tx.SystemFee);
            Assert.AreEqual(Engine.Transaction.ValidUntilBlock, tx.ValidUntilBlock);
            Assert.AreEqual(Engine.Transaction.Version, tx.Version);
            Assert.AreEqual(Engine.Transaction.Hash.ToString(), tx.Hash?.ToString());
            Assert.AreEqual(Engine.Sender.ToString(), tx.Sender?.ToString());
        }

        [TestMethod]
        public void Test_GetTransactionSystemFee()
        {
            Engine.Transaction.SystemFee = 12345_0000;
            Assert.AreEqual(Engine.Transaction.SystemFee, Contract.GetTransactionSystemFee());
        }

        [TestMethod]
        public void Test_GetTransactionNetworkFee()
        {
            Assert.AreEqual(Engine.Transaction.NetworkFee, Contract.GetTransactionNetworkFee());
        }

        [TestMethod]
        public void Test_GetTransactionValidUntilBlock()
        {
            Assert.AreEqual(Engine.Transaction.ValidUntilBlock, Contract.GetTransactionValidUntilBlock());
        }

        [TestMethod]
        public void Test_GetTransactionScript()
        {
            var script = Contract.GetTransactionScript();

            CollectionAssert.AreEqual(Engine.Transaction.Script.ToArray(), script);
        }
    }
}
