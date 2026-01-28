using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleNep17Token(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleNep17Token"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":19,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":21,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":59,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":321,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":748,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":799,""safe"":false},{""name"":""getMinter"",""parameters"":[],""returntype"":""Hash160"",""offset"":888,""safe"":true},{""name"":""setMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":933,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1015,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1064,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1116,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1122,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1167,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]},{""name"":""SetMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample NEP-17 token"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy45LjArNDIzNzFmNWY0ZTBiZTI4N2ExZmYyOGYzNThhYjI0NmY1YjQuLi4AAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUCAAAPAAD9xAQMEFNhbXBsZU5lcDE3VG9rZW5AGEAMAQBB9rRr4kGSXegxStgmBEVYQFcAAXgMAQBBm/ZnzkHmPxiEQFcBAXhK2SgkBkUJIgbKABSzqiYlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBkUQIgTbISICQErZKCQGRQkiBsoAFLNAEYhOEFHQUBLAQEGb9mfOQErYJgZFECIE2yFAwUVTi1BBkl3oMUBXAgJBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBkUQIgTbIXFpeZ5KcUVpELUmBQkiJWkQsyYQeGjBRVOLUEEvWMXtIg9peGjBRVOLUEHmPxiECCICQMFFU4tQQS9Yxe1AwUVTi1BB5j8YhEBXAQR4StkoJAZFCSIGygAUs6omJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGgLlyYFCCIReUrZKCQGRQkiBsoAFLOqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBQkiJnoQmCYYept4Ndz+//+qJgUJIhN6eTXP/v//RXt6eXg0DAgiAkBB+CfsjEBXAQR6eXgTwAwIVHJhbnNmZXJBlQFvYXlwaAuXqiQFCSILeTcAAHBoC5eqJh97engTwB8MDm9uTkVQMTdQYXltZW50eUFifVtSRUA3AABAQWJ9W1JAVwACeZkQtSYLDAZhbW91bnQ6eRCzJgQiIXl4NUn+//9FNYn9//95nko1lf3//0ULeXgLNXj///9AVwACeZkQtSYLDAZhbW91bnQ6eRCzJgQiMHmbeDUP/v//qiYODAlleGNlcHRpb246NUH9//95n0o1Tf3//0ULeQt4NTD///9AVwEADAH/2zBB1Y1e6HBoC5cmBVkiEGhK2CQJSsoAFCgDOiICQEHVjV7oQDTVQfgn7IxAVwABNPUJlyYWDBFObyBBdXRob3JpemF0aW9uITp4C5gkBQkiEHhK2SgkBkUJIgbKABSzJh94DAH/2zBBOQzjCngRwAwIU2V0T3duZXJBlQFvYUBBOQzjCkBXAQAMAf3bMEHVjV7ocGgLlyYFWiIQaErYJAlKygAUKAM6IgJANNtB+CfsjEBXAAE1b////wmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnhK2SgkBkUJIgbKABSzqiYEIiB4DAH92zBBOQzjCngRwAwJU2V0TWludGVyQZUBb2FAVwACNR3///8JlyQFCSIGNJcJlyYWDBFObyBBdXRob3JpemF0aW9uITp5eDVJ/v//QFcAAjXs/v//CZckBQkiCTVm////CZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1Tv7//0A1u/7//0BXAAI1sv7//wmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4NwEACCICQDcBAEBWAxBgEGAMFGKZMx5XjGZFMfS/kn/Y2DEr0jQ4YQwUYpkzHleMZkUx9L+Sf9jYMSvSNDhiQD+6I4E=").AsSerializable<Neo.SmartContract.NefFile>();

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
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract bool? Update(byte[]? nefFile, string? manifest);

    #endregion
}
