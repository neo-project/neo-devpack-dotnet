using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_CheckedUnchecked(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_CheckedUnchecked"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""addChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""addUnchecked"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":25,""safe"":false},{""name"":""castChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""castUnchecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":101,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIZXAAJ4eZ5KAgAAAIAuAzpKAv///38yAzpAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAF4ShAuAzpKA/////8AAAAAMgM6QFcAAXhKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQDsMdhk="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 03
    /// 000E : OpCode.THROW
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSHINT32 FFFFFF7F
    /// 0015 : OpCode.JMPLE 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.RET
    /// </remarks>
    [DisplayName("addChecked")]
    public abstract BigInteger? AddChecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0A
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 FFFFFF7F
    /// 0016 : OpCode.JMPLE 1E
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 FFFFFF7F
    /// 0028 : OpCode.JMPLE 0C
    /// 002A : OpCode.PUSHINT64 0000000001000000
    /// 0033 : OpCode.SUB
    /// 0034 : OpCode.RET
    /// </remarks>
    [DisplayName("addUnchecked")]
    public abstract BigInteger? AddUnchecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0013 : OpCode.JMPLE 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("castChecked")]
    public abstract BigInteger? CastChecked(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 04
    /// 0008 : OpCode.JMP 0E
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0014 : OpCode.JMPLE 0C
    /// 0016 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001F : OpCode.AND
    /// 0020 : OpCode.RET
    /// </remarks>
    [DisplayName("castUnchecked")]
    public abstract BigInteger? CastUnchecked(BigInteger? a);

    #endregion

}
