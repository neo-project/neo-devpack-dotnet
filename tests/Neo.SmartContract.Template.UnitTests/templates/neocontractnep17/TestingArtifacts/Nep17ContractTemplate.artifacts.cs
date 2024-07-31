using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":1136,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1151,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":43,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":89,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":275,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":691,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":733,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":853,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":893,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":933,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":939,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":957,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1081,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1120,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9jwRXAAEMB0VYQU1QTEVAVwABeDQDQFcAAXg0A0BXAAF4NANAVwABQFcAARhAWdgmFwwBAEH2tGviQZJd6DFK2CYERRBKYUBXAAF4YXgMAQBBm/ZnzkHmPxiEQFcBAXhwaAuXJgUIIg14StkoUMoAFLOrqiYlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFAVwICQZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshcWl5nkpxRWkQtSYFCSIjaRCzJhB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcBBHhwaAuXJgUIIg14StkoUMoAFLOrqiYkDB9UaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaAuXJgUIIg15StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IyqJgUJIiR6EJgmGHqbeDX0/v//qiYFCSIRenk15/7//0V7enl4NAQIQFcBBMJKeM9Kec9Kes8MCFRyYW5zZmVyQZUBb2F5cGgLl6okBQkiC3k3AABwaAuXqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwACeZkQtSYLDAZhbW91bnQ6eRCzJgQiIXl4NW7+//9FNdj9//95nko16v3//0ULeXgLNX3///9AVwACeZkQtSYLDAZhbW91bnQ6eRCzJgQiMHmbeDU0/v//qiYODAlleGNlcHRpb246NZD9//95n0o1ov3//0ULeQt4NTX///9ADAH/2zA0DkrYJAlKygAUKAM6QFcAAXhB9rRr4kGSXegxQDTeQfgn7IxAVwEBNPUJlyYWDBFObyBBdXRob3JpemF0aW9uITp4StkoUMoAFLOrJAUJIgZ4ELOqDBNvd25lciBtdXN0IGJlIHZhbGlk4TSQcHgMAf/bMDQZwkpoz0p4zwwIU2V0T3duZXJBlQFvYUBXAAJ5eEGb9mfOQeY/GIRAVwACNX3///8JlyYWDBFObyBBdXRob3JpemF0aW9uITp5eDX0/v//QFcAAjVV////CZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1k/7//0A1MP///0AMBUhlbGxvQZv2Z85Bkl3oMUBXAQJ5JgQidXhwaAuXJgxBLVEIMBPOSoBFeHBoStkoUMoAFLOrJAUJIgZoELOqDBFvd25lciBtdXN0IGV4aXN0c+FoDAH/2zA1QP///8JKC89KaM8MCFNldE93bmVyQZUBb2EMBVdvcmxkDAVIZWxsb0Gb9mfOQeY/GIRAVwADNZn+//8JlyYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AQBAVgIKxPv//wqZ+///EsBgQMJKWM9KNZj7//8jhvv//8JKWM9KNYn7//8jnfv//0BHmPIi"));

    #endregion

    #region Events

    [DisplayName("SetOwner")]
    public event Neo.SmartContract.Testing.TestingStandards.IOwnable.delSetOwner? OnSetOwner;

    [DisplayName("Transfer")]
    public event Neo.SmartContract.Testing.TestingStandards.INep17Standard.delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract bool? Verify { [DisplayName("verify")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
