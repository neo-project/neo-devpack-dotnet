using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Switch : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Switch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""switchLong"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""switch6"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":263,""safe"":false},{""name"":""switch6Inline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":333,""safe"":false},{""name"":""switchInteger"",""parameters"":[{""name"":""b"",""type"":""Integer""}],""returntype"":""Any"",""offset"":408,""safe"":false},{""name"":""switchLongLong"",""parameters"":[{""name"":""test"",""type"":""String""}],""returntype"":""Any"",""offset"":456,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2gA1cBAXhwaAwBMJcltgAAAGgMATGXJa8AAABoDAEylyWoAAAAaAwBM5cloQAAAGgMATSXJZoAAABoDAE1lyWTAAAAaAwBNpcljAAAAGgMATeXJYUAAABoDAE4lyV+AAAAaAwBOZckd2gMAjEwlyRyaAwCMTGXJG1oDAIxMpckaGgMAjEzlyRjaAwCMTSXJF5oDAIxNZckWWgMAjE2lyRUaAwCMTeXJFBoDAIxOJckTGgMAjE5lyRIaAwCMjCXJEQiRhEiRRIiQhMiPxQiPBUiORYiNhciMxgiMBkiLRoiKhsiJxwiJB0iIR4iHh8iGyAiGAARIhQAEiIQABMiDAAUIggAFSIEAGNAVwEBeHBoDAEwlyQnaAwBMZckI2gMATKXJB9oDAEzlyQbaAwBNJckF2gMATWXJBMiFBEiExIiEBMiDRQiChUiBxYiBABjQFcBAXhwaAwBMJcmBREiPWgMATGXJgUSIjNoDAEylyYFEyIpaAwBM5cmBRQiH2gMATSXJgUVIhVoDAE1lyYFFiILCCYGAGMiBGg6QFcCARFweHFpEZckDmkSlyQPaROXJBAiFBJKcEUiEhNKcEUiDBZKcEUiBhBKcEVoQFcCARFweHFpDAFhlyRAaQwBY5ckcWkMAWKXJaMAAABpDAFklyXRAAAAaQwBZZclAAEAAGkMAWaXJSwBAABpDAFnlyVYAQAAI4kBAABoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEUjVwEAAGgSoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEUjHgEAAGhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSPmAAAAaA+gSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRSOtAAAAaGigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRSJ0aBOgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRSI+aBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRSIIaBGhSnBFaEC9xIaY"));

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
