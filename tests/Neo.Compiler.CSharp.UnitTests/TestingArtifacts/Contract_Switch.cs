using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Switch : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Switch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""switchLong"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""switch6"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":265,""safe"":false},{""name"":""switch6Inline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":337,""safe"":false},{""name"":""switchInteger"",""parameters"":[{""name"":""b"",""type"":""Integer""}],""returntype"":""Any"",""offset"":414,""safe"":false},{""name"":""switchLongLong"",""parameters"":[{""name"":""test"",""type"":""String""}],""returntype"":""Any"",""offset"":466,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9rgNXAQF4cGgMATCXJbYAAABoDAExlyWvAAAAaAwBMpclqAAAAGgMATOXJaEAAABoDAE0lyWaAAAAaAwBNZclkwAAAGgMATaXJYwAAABoDAE3lyWFAAAAaAwBOJclfgAAAGgMATmXJHdoDAIxMJckcmgMAjExlyRtaAwCMTKXJGhoDAIxM5ckY2gMAjE0lyReaAwCMTWXJFloDAIxNpckVGgMAjE3lyRQaAwCMTiXJExoDAIxOZckSGgMAjIwlyREIkYRIkcSIkQTIkEUIj4VIjsWIjgXIjUYIjIZIi8aIiwbIikcIiYdIiMeIiAfIh0gIhoAESIWABIiEgATIg4AFCIKABUiBgBjIgJAVwEBeHBoDAEwlyQnaAwBMZckI2gMATKXJB9oDAEzlyQbaAwBNJckF2gMATWXJBMiFBEiFRIiEhMiDxQiDBUiCRYiBgBjIgJAVwEBeHBoDAEwlyYFESI9aAwBMZcmBRIiM2gMATKXJgUTIiloDAEzlyYFFCIfaAwBNJcmBRUiFWgMATWXJgUWIgsIJgYAYyIEaDoiAkBXAgERcHhxaRGXJA5pEpckD2kTlyQQIhQSSnBFIhQTSnBFIg4WSnBFIggQSnBFIgJoIgJAVwIBEXB4cWkMAWGXJEBpDAFjlyRxaQwBYpclowAAAGkMAWSXJdEAAABpDAFllyUAAQAAaQwBZpclLAEAAGkMAWeXJVgBAAAjiQEAAGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSNZAQAAaBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRSMgAQAAaEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFI+gAAABoD6BKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFI68AAABoaKBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFInZoE6BKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFIkBoEp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFIgpoEaFKcEUiAmgiAkBEB5Zt"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switch6")]
    public abstract object? Switch6(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switch6Inline")]
    public abstract object? Switch6Inline(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switchInteger")]
    public abstract object? SwitchInteger(BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switchLong")]
    public abstract object? SwitchLong(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switchLongLong")]
    public abstract object? SwitchLongLong(string? test);

    #endregion

    #region Constructor for internal use only

    protected Contract_Switch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
