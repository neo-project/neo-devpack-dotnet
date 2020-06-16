using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Enumerators;
using Neo.VM.Types;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class NFTTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(TriggerType.Application, new TestScriptContainer(), null);
            _engine.AddEntryScript("../../../../../templates/Template.NFT.CSharp/NFTContract.cs");
        }

        [TestMethod]
        public void Test_GetName()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("name").Pop();

            StackItem wantResult = "NFT";
            Assert.AreEqual(wantResult.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_GetStandards()
        {
            _engine.Reset();
            var result = (Array)_engine.ExecuteTestCaseStandard("supportedStandards").Pop();

            Assert.AreEqual(2, result.Count);
            StackItem wantResult1 = "NEP-10";
            StackItem wantResult2 = "NEP-11";
            Assert.AreEqual(wantResult1.ConvertTo(StackItemType.ByteString), result[0]);
            Assert.AreEqual(wantResult2.ConvertTo(StackItemType.ByteString), result[1]);
        }

        [TestMethod]
        public void Test_GetDecimals()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("decimals").Pop();

            var wantResult = 8;
            Assert.AreEqual(wantResult, result as Integer);
        }

        [TestMethod]
        public void Test_MintNFT()
        {
            var hash = _engine.CurrentScriptHash;
            var snapshot = _engine.Snapshot as TestSnapshot;

            snapshot.Contracts.Add(hash, new Ledger.ContractState()
            {
                Manifest = new Manifest.ContractManifest()
                {
                    Features = Manifest.ContractFeatures.HasStorage
                }
            });

            var tokenid = System.Text.Encoding.Default.GetBytes("abc");
            var owner = Wallets.Helper.ToScriptHash("Nj9Epc1x2sDmd6yH5qJPYwXRqSRf5X6KHE").ToArray();
            var to = Wallets.Helper.ToScriptHash("NTegNkUTqL5UUqb5MjsHP4cbXftkhuZA1p").ToArray();
            var properties = "NFT properties";

            // Mint NFT
            var result = _engine.ExecuteTestCaseStandard("mint", tokenid, owner, properties).Pop();
            var wantResult = true;
            Assert.AreEqual(wantResult, result.ConvertTo(StackItemType.Boolean));

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("totalSupply").Pop();
            var wantTotalSupply = BigInteger.Pow(10, 8);
            Assert.AreEqual(wantTotalSupply, result);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("properties", tokenid).Pop();
            Assert.AreEqual(properties, result);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("balanceOf", owner, Null.Null).Pop();
            var wantBalance = new Integer(BigInteger.Pow(10, 8));
            Assert.AreEqual(wantBalance, result);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("balanceOf", owner, tokenid).Pop();
            wantBalance = BigInteger.Pow(10, 8);
            Assert.AreEqual(wantBalance, result);

            // Mint new NFT
            _engine.Reset();
            var tokenid2 = System.Text.Encoding.Default.GetBytes("def");
            result = _engine.ExecuteTestCaseStandard("mint", tokenid2, owner, properties).Pop();
            wantResult = true;
            Assert.AreEqual(wantResult, result.ConvertTo(StackItemType.Boolean));

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("totalSupply").Pop();
            wantTotalSupply = 2 * BigInteger.Pow(10, 8);
            Assert.AreEqual(wantTotalSupply, result);

            // Balance of all
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("balanceOf", owner, Null.Null).Pop();
            wantBalance = 2 * BigInteger.Pow(10, 8);
            Assert.AreEqual(wantBalance, result);

            // Balance of token2
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("balanceOf", owner, tokenid2).Pop();
            wantBalance = BigInteger.Pow(10, 8);
            Assert.AreEqual(wantBalance, result);

            // Transfer
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("transfer", owner, to, BigInteger.Pow(10, 7), tokenid2).Pop();
            Assert.AreEqual(true, result.ConvertTo(StackItemType.Boolean));

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("balanceOf", owner, tokenid2).Pop();
            wantBalance = 9 * BigInteger.Pow(10, 7);
            Assert.AreEqual(wantBalance, result);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("balanceOf", to, tokenid2).Pop();
            wantBalance = BigInteger.Pow(10, 7);
            Assert.AreEqual(wantBalance, result);

            // OwnerOf
            _engine.Reset();
            var enumerator = ((InteropInterface)_engine.ExecuteTestCaseStandard("ownerOf", tokenid2).Pop()).GetInterface<IEnumerator>();
            enumerator.Next();
            var v1 = enumerator.Value();
            Assert.AreEqual(new ByteString(to), v1);

            enumerator.Next();
            var v2 = enumerator.Value();
            Assert.AreEqual(new ByteString(owner), v2);

            // TokensOf
            _engine.Reset();
            enumerator = ((InteropInterface)_engine.ExecuteTestCaseStandard("tokensOf", owner).Pop()).GetInterface<IEnumerator>();
            enumerator.Next();
            v1 = enumerator.Value();
            Assert.AreEqual(new ByteString(tokenid), v1);

            enumerator.Next();
            v2 = enumerator.Value();
            Assert.AreEqual(new ByteString(tokenid2), v2);

            // BurnToken
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("burn", tokenid2, owner, BigInteger.Pow(10, 7)).Pop();
            Assert.AreEqual(true, result.ConvertTo(StackItemType.Boolean));

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("balanceOf", owner, tokenid2).Pop();
            wantBalance = 8 * BigInteger.Pow(10, 7);
            Assert.AreEqual(wantBalance, result);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("totalSupply").Pop();
            wantBalance = 19 * BigInteger.Pow(10, 7);
            Assert.AreEqual(wantBalance, result);
        }
    }

    internal class TestScriptContainer : IVerifiable
    {
        public Witness[] Witnesses { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public int Size => throw new System.NotImplementedException();

        public void Deserialize(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void DeserializeUnsigned(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        public UInt160[] GetScriptHashesForVerifying(StoreView snapshot)
        {
            var hash = Wallets.Helper.ToScriptHash("Nj9Epc1x2sDmd6yH5qJPYwXRqSRf5X6KHE");
            return new UInt160[] { hash };
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void SerializeUnsigned(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
