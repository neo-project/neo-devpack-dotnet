using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard17Enum : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.INep17Standard
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard17Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":574,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":592,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":57,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":253,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":610,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":558,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABcaHR0cHM6Ly9naXRodWIuY29tL25lby1wcm9qZWN0L25lby1kZXZwYWNrLWRvdG5ldC90cmVlL21hc3Rlci9zcmMvTmVvLlNtYXJ0Q29udHJhY3QuVGVtcGxhdGUAAf2j+kNG6lMqJY/El92t22Q3yf3/C2dldENvbnRyYWN0AQABDwAA/XECEM5AVwABeDQDQFcAAXg0A0BXAAF4NANAVwABQBHOQFnYJhcMAQBB9rRr4kGSXegxStgmBEUQSmFAVwEBeHBoC5cmBxHbICINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshIgJAVwICQZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshcWl5nkpxRWkQtSYHENsgIidpELMmEHhowUVTi1BBL1jF7SIPaXhowUVTi1BB5j8YhBHbICICQFcBBHhwaAuXJgcR2yAiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBoC5cmBxHbICINeUrZKFDKABSzq6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMqiYHENsgIip6EJgmGnqbeDXo/v//qiYHENsgIhV6eTXZ/v//RXt6eXg0CBHbICICQFcBBMJKeM9Kec9Kes8MCFRyYW5zZmVyQZUBb2F5cGgLl6okBxDbICILeTcAAHBoC5eqJh97engTwB8MDm9uTkVQMTdQYXltZW50eUFifVtSRUBXAARAVgIK7P3//wrL/f//EsBgQBALEsBKWM9KNb39//8jtf3//xALEsBKWM9KNav9//8jv/3//xALEsBKWM9KNZn9//8iu8m/he4="));

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
