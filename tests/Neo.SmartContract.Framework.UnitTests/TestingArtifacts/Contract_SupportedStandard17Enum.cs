using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard17Enum : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.INep17Standard
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard17Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":574,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":592,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":57,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":253,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":610,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":558,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP1xAhDOQFcAAXg0A0BXAAF4NANAVwABeDQDQFcAAUARzkBZ2CYXDAEAQfa0a+JBkl3oMUrYJgRFEEphQFcBAXhwaAuXJgcR2yAiDXhK2ShQygAUs6uqJiUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbISICQFcCAkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIXFpeZ5KcUVpELUmBxDbICInaRCzJhB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQR2yAiAkBXAQR4cGgLlyYHEdsgIg14StkoUMoAFLOrqiYkDB9UaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaAuXJgcR2yAiDXlK2ShQygAUs6uqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBxDbICIqehCYJhp6m3g16P7//6omBxDbICIVenk12f7//0V7enl4NAgR2yAiAkBXAQTCSnjPSnnPSnrPDAhUcmFuc2ZlckGVAW9heXBoC5eqJAcQ2yAiC3k3AABwaAuXqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwAEQFYCCuz9//8Ky/3//xLAYEAQCxLASljPSjW9/f//I7X9//8QCxLASljPSjWr/f//I7/9//8QCxLASljPSjWZ/f//IrtLSH0D"));

    #endregion

    #region Events

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
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

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
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected Contract_SupportedStandard17Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
