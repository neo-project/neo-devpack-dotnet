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
    /// Script: VwACeHmeSgIAAACALgM6SgL///9/MgM6QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 03
    /// 0E : OpCode.THROW
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSHINT32 FFFFFF7F
    /// 15 : OpCode.JMPLE 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.RET
    /// </remarks>
    [DisplayName("addChecked")]
    public abstract BigInteger? AddChecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.RET
    /// </remarks>
    [DisplayName("addUnchecked")]
    public abstract BigInteger? AddUnchecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6SgP/////AAAAADIDOkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 13 : OpCode.JMPLE 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("castChecked")]
    public abstract BigInteger? CastChecked(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 04
    /// 08 : OpCode.JMP 0E
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 14 : OpCode.JMPLE 0C
    /// 16 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1F : OpCode.AND
    /// 20 : OpCode.RET
    /// </remarks>
    [DisplayName("castUnchecked")]
    public abstract BigInteger? CastUnchecked(BigInteger? a);

    #endregion
}
