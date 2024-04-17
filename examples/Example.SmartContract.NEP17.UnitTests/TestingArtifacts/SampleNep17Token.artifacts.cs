using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleNep17Token : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IVerificable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleNep17Token"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":1333,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1348,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":52,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":98,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":362,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":808,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Any""}],""returntype"":""Void"",""offset"":877,""safe"":false},{""name"":""getMinter"",""parameters"":[],""returntype"":""Hash160"",""offset"":980,""safe"":true},{""name"":""setMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1025,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1103,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1158,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1216,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1222,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1271,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]},{""name"":""SetMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample NEP-17 token"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy42LjIrZmFiMWEyZWVhZGYyMTE2NjhiMjg0ZWZiYTgwYzFhNTU3ZTYuLi4AAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUCAAAPAAD9UwVXAAEMEFNhbXBsZU5lcDE3VG9rZW5AVwABeDQDQFcAAXg0A0BXAAF4NANAVwABQFcAARhAWdgmFwwBAEH2tGviQZJd6DFK2CYERRBKYUBXAAF4YXgMAQBBm/ZnzkHmPxiEQFcBAXhwaAuXJgcR2yAiDXhK2ShQygAUs6uqJiUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbISICQErZKFDKABSzq0ARiE4QUdBQEsBAQZv2Z85AStgmBEUQ2yFAwUVTi1BBkl3oMUBXAgJBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFxaXmeSnFFaRC1JgcQ2yAiJ2kQsyYQeGjBRVOLUEEvWMXtIg9peGjBRVOLUEHmPxiEEdsgIgJAwUVTi1BBL1jF7UDBRVOLUEHmPxiEQFcBBHhwaAuXJgcR2yAiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBoC5cmBxHbICINeUrZKFDKABSzq6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMqiYHENsgIip6EJgmGnqbeDXS/v//qiYHENsgIhV6eTXD/v//RXt6eXg0DhHbICICQEH4J+yMQFcBBMJKeM9Kec9Kes8MCFRyYW5zZmVyQZUBb2F5cGgLl6okBxDbICILeTcAAHBoC5eqJh97engTwB8MDm9uTkVQMTdQYXltZW50eUFifVtSRUA3AABAQWJ9W1JAVwACeZkQtSYLDAZhbW91bnQ6eRCzJgQiIXl4NTT+//9FNWz9//95nko1fv3//0ULeXgLNXH///9AVwACeZkQtSYLDAZhbW91bnQ6eRCzJgQiMHmbeDX6/f//qiYODAlleGNlcHRpb246NST9//95n0o1Nv3//0ULeQt4NSn///9AVwEADAH/2zA0GnBoC5cmBVoiEGhK2CQJSsoAFCgDOiICQFcAAXhB9rRr4kGSXegxQEGSXegxQEH2tGviQDTDQfgn7IxAVwABNPUQ2yCXJhYMEU5vIEF1dGhvcml6YXRpb24hOngLmCQHENsgIgx4StkoUMoAFLOrJh14DAH/2zA0FsJKeM8MCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQEHmPxiEQFcBAAwB/dswNW7///9waAuXJgVbIhBoStgkCUrKABQoAzoiAkA020H4J+yMQFcAATVh////ENsglyYWDBFObyBBdXRob3JpemF0aW9uITp4StkoUMoAFLOrqiYEIh54DAH92zA0hsJKeM8MCVNldE1pbnRlckGVAW9hQFcAAjUT////ENsglyQHENsgIgg0lxDbIJcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1J/7//0BXAAI13P7//xDbIJckBxDbICILNWD///8Q2yCXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4NSb+//9ANaX+//9AVwACNZz+//8Q2yCXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4NwEAEdsgIgJANwEAQFYEDBRimTMeV4xmRTH0v5J/2NgxK9I0OGIMFGKZMx5XjGZFMfS/kn/Y2DEr0jQ4YwoI+///CtT6//8SwGBAwkpYz0o13Pr//yPB+v//wkpYz0o1zfr//yPh+v//almniA=="));

    #endregion

    #region Events

    public delegate void delSetMinter(UInt160? newMinter);

    [DisplayName("SetMinter")]
    public event delSetMinter? OnSetMinter;

    public delegate void delSetOwner(UInt160? newOwner);

    [DisplayName("SetOwner")]
    public event delSetOwner? OnSetOwner;

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
    public abstract UInt160? Minter { [DisplayName("getMinter")] get; [DisplayName("setMinter")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; }

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
    [DisplayName("setOwner")]
    public abstract void SetOwner(object? newOwner = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract bool? Update(byte[]? nefFile, string? manifest);

    #endregion

    #region Constructor for internal use only

    protected SampleNep17Token(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
