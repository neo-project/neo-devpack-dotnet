using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Extensions;

namespace Neo.SmartContract.Testing.UnitTests.Extensions
{
    [TestClass]
    public class ArtifactExtensionsTests
    {
        [TestMethod]
        public void TestGetArtifactsSource()
        {
            var manifest = ContractManifest.FromJson(JToken.Parse(
                @"{""name"":""Contract1"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":1406,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1421,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":43,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":85,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":281,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":711,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":755,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":873,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":915,""safe"":false},{""name"":""withdraw"",""parameters"":[{""name"":""token"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":957,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1139,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1203,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":1209,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":1229,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":1352,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1390,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E""}}") as JObject);

            // Create artifacts

            var source = manifest.GetArtifactsSource(manifest.Name, generateProperties: true);

            Assert.AreEqual(source, @"
using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract1 : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IVerificable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@""{""""name"""":""""Contract1"""",""""groups"""":[],""""features"""":{},""""supportedstandards"""":[""""NEP-17""""],""""abi"""":{""""methods"""":[{""""name"""":""""symbol"""",""""parameters"""":[],""""returntype"""":""""String"""",""""offset"""":1406,""""safe"""":true},{""""name"""":""""decimals"""",""""parameters"""":[],""""returntype"""":""""Integer"""",""""offset"""":1421,""""safe"""":true},{""""name"""":""""totalSupply"""",""""parameters"""":[],""""returntype"""":""""Integer"""",""""offset"""":43,""""safe"""":true},{""""name"""":""""balanceOf"""",""""parameters"""":[{""""name"""":""""owner"""",""""type"""":""""Hash160""""}],""""returntype"""":""""Integer"""",""""offset"""":85,""""safe"""":true},{""""name"""":""""transfer"""",""""parameters"""":[{""""name"""":""""from"""",""""type"""":""""Hash160""""},{""""name"""":""""to"""",""""type"""":""""Hash160""""},{""""name"""":""""amount"""",""""type"""":""""Integer""""},{""""name"""":""""data"""",""""type"""":""""Any""""}],""""returntype"""":""""Boolean"""",""""offset"""":281,""""safe"""":false},{""""name"""":""""getOwner"""",""""parameters"""":[],""""returntype"""":""""Hash160"""",""""offset"""":711,""""safe"""":true},{""""name"""":""""setOwner"""",""""parameters"""":[{""""name"""":""""newOwner"""",""""type"""":""""Hash160""""}],""""returntype"""":""""Void"""",""""offset"""":755,""""safe"""":false},{""""name"""":""""burn"""",""""parameters"""":[{""""name"""":""""account"""",""""type"""":""""Hash160""""},{""""name"""":""""amount"""",""""type"""":""""Integer""""}],""""returntype"""":""""Void"""",""""offset"""":873,""""safe"""":false},{""""name"""":""""mint"""",""""parameters"""":[{""""name"""":""""to"""",""""type"""":""""Hash160""""},{""""name"""":""""amount"""",""""type"""":""""Integer""""}],""""returntype"""":""""Void"""",""""offset"""":915,""""safe"""":false},{""""name"""":""""withdraw"""",""""parameters"""":[{""""name"""":""""token"""",""""type"""":""""Hash160""""},{""""name"""":""""to"""",""""type"""":""""Hash160""""},{""""name"""":""""amount"""",""""type"""":""""Integer""""}],""""returntype"""":""""Boolean"""",""""offset"""":957,""""safe"""":false},{""""name"""":""""onNEP17Payment"""",""""parameters"""":[{""""name"""":""""from"""",""""type"""":""""Hash160""""},{""""name"""":""""amount"""",""""type"""":""""Integer""""},{""""name"""":""""data"""",""""type"""":""""Any""""}],""""returntype"""":""""Void"""",""""offset"""":1139,""""safe"""":false},{""""name"""":""""verify"""",""""parameters"""":[],""""returntype"""":""""Boolean"""",""""offset"""":1203,""""safe"""":true},{""""name"""":""""myMethod"""",""""parameters"""":[],""""returntype"""":""""String"""",""""offset"""":1209,""""safe"""":false},{""""name"""":""""_deploy"""",""""parameters"""":[{""""name"""":""""data"""",""""type"""":""""Any""""},{""""name"""":""""update"""",""""type"""":""""Boolean""""}],""""returntype"""":""""Void"""",""""offset"""":1229,""""safe"""":false},{""""name"""":""""update"""",""""parameters"""":[{""""name"""":""""nefFile"""",""""type"""":""""ByteArray""""},{""""name"""":""""manifest"""",""""type"""":""""String""""}],""""returntype"""":""""Void"""",""""offset"""":1352,""""safe"""":false},{""""name"""":""""_initialize"""",""""parameters"""":[],""""returntype"""":""""Void"""",""""offset"""":1390,""""safe"""":false}],""""events"""":[{""""name"""":""""Transfer"""",""""parameters"""":[{""""name"""":""""from"""",""""type"""":""""Hash160""""},{""""name"""":""""to"""",""""type"""":""""Hash160""""},{""""name"""":""""amount"""",""""type"""":""""Integer""""}]},{""""name"""":""""SetOwner"""",""""parameters"""":[{""""name"""":""""newOwner"""",""""type"""":""""Hash160""""}]}]},""""permissions"""":[{""""contract"""":""""*"""",""""methods"""":""""*""""}],""""trusts"""":[],""""extra"""":{""""Author"""":""""\u003CYour Name Or Company Here\u003E"""",""""Description"""":""""\u003CDescription Here\u003E"""",""""Email"""":""""\u003CYour Public Email Here\u003E"""",""""Version"""":""""\u003CVersion String Here\u003E""""}}"");

    #endregion

    #region Events

    public delegate void delSetOwner(UInt160? newOwner);

    [DisplayName(""SetOwner"")]
    public event delSetOwner? OnSetOwner;

    [DisplayName(""Transfer"")]
    public event Neo.SmartContract.Testing.TestingStandards.INep17Standard.delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName(""decimals"")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName(""getOwner"")] get; [DisplayName(""setOwner"")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName(""symbol"")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName(""totalSupply"")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract bool? Verify { [DisplayName(""verify"")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName(""balanceOf"")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName(""burn"")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName(""mint"")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName(""myMethod"")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName(""onNEP17Payment"")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName(""transfer"")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName(""update"")]
    public abstract void Update(byte[]? nefFile, string? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName(""withdraw"")]
    public abstract bool? Withdraw(UInt160? token, UInt160? to, BigInteger? amount);

    #endregion

    #region Constructor for internal use only

    protected Contract1(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
".Replace("\r\n", "\n").TrimStart());
        }
    }
}
