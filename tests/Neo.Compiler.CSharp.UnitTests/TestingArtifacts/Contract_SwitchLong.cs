using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SwitchLong : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SwitchLong"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9CQFXAQF4cGgMATCXJbYAAABoDAExlyWvAAAAaAwBMpclqAAAAGgMATOXJaEAAABoDAE0lyWaAAAAaAwBNZclkwAAAGgMATaXJYwAAABoDAE3lyWFAAAAaAwBOJclfgAAAGgMATmXJHdoDAIxMJckcmgMAjExlyRtaAwCMTKXJGhoDAIxM5ckY2gMAjE0lyReaAwCMTWXJFloDAIxNpckVGgMAjE3lyRQaAwCMTiXJExoDAIxOZckSGgMAjIwlyREIkYRIkcSIkQTIkEUIj4VIjsWIjgXIjUYIjIZIi8aIiwbIikcIiYdIiMeIiAfIh0gIhoAESIWABIiEgATIg4AFCIKABUiBgBjIgJAIpMHQA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMain")]
    public abstract object? TestMain(string? method);

    #endregion

    #region Constructor for internal use only

    protected Contract_SwitchLong(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
