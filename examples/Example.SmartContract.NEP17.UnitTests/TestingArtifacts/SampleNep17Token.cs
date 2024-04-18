using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleNep17Token : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IVerificable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleNep17Token"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":1227,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1242,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":52,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":98,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":294,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":724,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Any""}],""returntype"":""Void"",""offset"":781,""safe"":false},{""name"":""getMinter"",""parameters"":[],""returntype"":""Hash160"",""offset"":878,""safe"":true},{""name"":""setMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":923,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1001,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1056,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1114,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1120,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1165,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]},{""name"":""SetMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample NEP-17 token"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUCAAAPAAD96QRXAAEMEFNhbXBsZU5lcDE3VG9rZW5AVwABeDQDQFcAAXg0A0BXAAF4NANAVwABQFcAARhAWdgmFwwBAEH2tGviQZJd6DFK2CYERRBKYUBXAAF4YXgMAQBBm/ZnzkHmPxiEQFcBAXhwaAuXJgcR2yAiDXhK2ShQygAUs6uqJiUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbISICQFcCAkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIXFpeZ5KcUVpELUmBxDbICInaRCzJhB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQR2yAiAkBXAQR4cGgLlyYHEdsgIg14StkoUMoAFLOrqiYkDB9UaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaAuXJgcR2yAiDXlK2ShQygAUs6uqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBxDbICIqehCYJhp6m3g16P7//6omBxDbICIVenk12f7//0V7enl4NAgR2yAiAkBXAQTCSnjPSnnPSnrPDAhUcmFuc2ZlckGVAW9heXBoC5eqJAcQ2yAiC3k3AABwaAuXqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwACeZkQtSYLDAZhbW91bnQ6eRCzJgQiIXl4NVr+//9FNcD9//95nko10v3//0ULeXgLNXv///9AVwACeZkQtSYLDAZhbW91bnQ6eRCzJgQiMHmbeDUg/v//qiYODAlleGNlcHRpb246NXj9//95n0o1iv3//0ULeQt4NTP///9AVwEADAH/2zA0GnBoC5cmBVoiEGhK2CQJSsoAFCgDOiICQFcAAXhB9rRr4kGSXegxQDTPQfgn7IxAVwABNPUQ2yCXJhYMEU5vIEF1dGhvcml6YXRpb24hOngLmCQHENsgIgx4StkoUMoAFLOrJh14DAH/2zA0FsJKeM8MCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQFcBAAwB/dswNYD///9waAuXJgVbIhBoStgkCUrKABQoAzoiAkA020H4J+yMQFcAATVn////ENsglyYWDBFObyBBdXRob3JpemF0aW9uITp4StkoUMoAFLOrqiYEIh54DAH92zA0jMJKeM8MCVNldE1pbnRlckGVAW9hQFcAAjUZ////ENsglyQHENsgIgg0lxDbIJcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1Of7//0BXAAI14v7//xDbIJckBxDbICILNWD///8Q2yCXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4NTj+//9ANav+//9AVwACNaL+//8Q2yCXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4NwEAEdsgIgJAVgQMFGKZMx5XjGZFMfS/kn/Y2DEr0jQ4YgwUYpkzHleMZkUx9L+Sf9jYMSvSNDhjCnL7//8KPvv//xLAYEDCSljPSjVG+///Iyv7///CSljPSjU3+///I0v7//+S4KkY"));

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
