using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_shift(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_shift"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testShift"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testShiftBigInt"",""parameters"":[],""returntype"":""Array"",""offset"":58,""safe"":false},{""name"":""shiftLeftChecked"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":87,""safe"":false},{""name"":""shiftLeftUnchecked"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":130,""safe"":false},{""name"":""shiftLeftCheckedLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":173,""safe"":false},{""name"":""shiftLeftCheckedByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":236,""safe"":false},{""name"":""shiftLeftCheckedShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":279,""safe"":false},{""name"":""shiftLeftBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":322,""safe"":false},{""name"":""shiftRightChecked"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":329,""safe"":false},{""name"":""shiftRightUnchecked"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":341,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1hAVcDABhwaBEAH5GoSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWgRAB+RqXJpalASwEBXBQAYcGgQqHFoEahyaBGpc2gSqXRpamtsVBTAQFcAAnh5AB+RqErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ4eQAfkahKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeHkAP5GoSsoYMjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9AVwACeHkAH5GoSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnh5AB+RqErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ4eahAVwACeHkAH5GpSoBAVwACeHkAH5GpSoBAybfNRQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmoQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// SHL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftLeftBigInteger")]
    public abstract BigInteger? ShiftLeftBigInteger(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GoSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftLeftChecked")]
    public abstract BigInteger? ShiftLeftChecked(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GoSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftLeftCheckedByte")]
    public abstract BigInteger? ShiftLeftCheckedByte(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5GoSsoYMjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH8 [1 datoshi]
    /// JMPLE 32 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftLeftCheckedLong")]
    public abstract BigInteger? ShiftLeftCheckedLong(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GoSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftLeftCheckedShort")]
    public abstract BigInteger? ShiftLeftCheckedShort(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GoSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftLeftUnchecked")]
    public abstract BigInteger? ShiftLeftUnchecked(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GpSoBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// DUP [2 datoshi]
    /// STARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftRightChecked")]
    public abstract BigInteger? ShiftRightChecked(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GpSoBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// DUP [2 datoshi]
    /// STARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftRightUnchecked")]
    public abstract BigInteger? ShiftRightUnchecked(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAGHBoEQAfkahKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaBEAH5GpcmlqUBLAQA==
    /// INITSLOT 0300 [64 datoshi]
    /// PUSH8 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShift")]
    public abstract IList<object>? TestShift();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAGHBoEKhxaBGocmgRqXNoEql0aWprbFQUwEA=
    /// INITSLOT 0500 [64 datoshi]
    /// PUSH8 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// SHL [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHL [8 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// SHR [8 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShiftBigInt")]
    public abstract IList<object>? TestShiftBigInt();

    #endregion
}
