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
    [DisplayName("addChecked")]
    public abstract BigInteger? AddChecked(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : ADD
    // 0006 : DUP
    // 0007 : PUSHINT32
    // 000C : JMPGE
    // 000E : THROW
    // 000F : DUP
    // 0010 : PUSHINT32
    // 0015 : JMPLE
    // 0017 : THROW
    // 0018 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("addUnchecked")]
    public abstract BigInteger? AddUnchecked(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : ADD
    // 0006 : DUP
    // 0007 : PUSHINT32
    // 000C : JMPGE
    // 000E : JMP
    // 0010 : DUP
    // 0011 : PUSHINT32
    // 0016 : JMPLE
    // 0018 : PUSHINT64
    // 0021 : AND
    // 0022 : DUP
    // 0023 : PUSHINT32
    // 0028 : JMPLE
    // 002A : PUSHINT64
    // 0033 : SUB
    // 0034 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("castChecked")]
    public abstract BigInteger? CastChecked(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSHINT64
    // 0013 : JMPLE
    // 0015 : THROW
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("castUnchecked")]
    public abstract BigInteger? CastUnchecked(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : JMP
    // 000A : DUP
    // 000B : PUSHINT64
    // 0014 : JMPLE
    // 0016 : PUSHINT64
    // 001F : AND
    // 0020 : RET

    #endregion

}
