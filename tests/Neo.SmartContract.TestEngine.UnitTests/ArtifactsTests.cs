using Neo.Json;
using Neo.SmartContract.Manifest;
using Neo.TestEngine.Contracts;

namespace Neo.SmartContract.TestEngine.UnitTests
{
    public class ArtifactsTests
    {
        //[Test]
        public void GenerateNativeArtifacts()
        {
            var manifest = Native.NativeContract.ContractManagement.Manifest;
            var source = Artifacts.CreateSourceFromManifest(manifest.Name, manifest.Abi).Replace("\r\n", "\n").Trim();
        }

        [Test]
        public void TestNativeContracts()
        {
            TestEngine engine = new();
            Assert.That(engine.Native.ContractManagement.Hash, Is.EqualTo(Native.NativeContract.ContractManagement.Hash));
        }

        [Test]
        public void TestCreateSourceFromManifest()
        {
            var manifest = ContractManifest.FromJson(JToken.Parse(
                @"{""name"":""Contract1"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":1406,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1421,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":43,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":85,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":281,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":711,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":755,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":873,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":915,""safe"":false},{""name"":""withdraw"",""parameters"":[{""name"":""token"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":957,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1139,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1203,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":1209,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":1229,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":1352,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1390,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E""}}") as JObject);

            // Create artifacts

            var source = Artifacts.CreateSourceFromManifest(manifest.Name, manifest.Abi).Replace("\r\n", "\n").Trim();

            Assert.That(source, Is.EqualTo(@"
using System.Collections.Generic;
using System.Numerics;

namespace Neo.TestEngine.Contracts;

public abstract class Contract1 : Neo.SmartContract.TestEngine.Mocks.SmartContract
{
#region Events
    public delegate void delSetOwner(UInt160 newOwner);
    public event delSetOwner? SetOwner;
    public delegate void delTransfer(UInt160 from, UInt160 to, BigInteger amount);
    public event delTransfer? Transfer;
#endregion
#region Safe methods
    public abstract BigInteger balanceOf(UInt160 owner);
    public abstract BigInteger decimals();
    public abstract UInt160 getOwner();
    public abstract string symbol();
    public abstract BigInteger totalSupply();
    public abstract bool verify();
#endregion
#region Unsafe methods
    public abstract void burn(UInt160 account, BigInteger amount);
    public abstract void mint(UInt160 to, BigInteger amount);
    public abstract string myMethod();
    public abstract void onNEP17Payment(UInt160 from, BigInteger amount, object data);
    public abstract void setOwner(UInt160 newOwner);
    public abstract bool transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
    public abstract void update(byte[] nefFile, string manifest);
    public abstract bool withdraw(UInt160 token, UInt160 to, BigInteger amount);
#endregion
#region Constructor for internal use only
    protected Contract1(Neo.SmartContract.TestEngine.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
#endregion
}
".Replace("\r\n", "\n").Trim()));
        }

        [Test]
        public void FromHashTest()
        {
            UInt160 hash = UInt160.Parse("0x1230000000000000000000000000000000000000");
            TestEngine engine = new();

            var contract = engine.FromHash<Contract1>(hash);

            Assert.That(contract.Hash, Is.EqualTo(hash));
        }
    }
}
